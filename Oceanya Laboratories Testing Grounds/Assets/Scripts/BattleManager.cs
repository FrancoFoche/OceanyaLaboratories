using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    Start,
    AllyPhase,
    EnemyPhase,
    Won,
    Lost
}

public enum TurnState
{
    WaitingForAction,
    WaitingForTarget,
    End
}

public class BattleManager : MonoBehaviour
{
    public BattleState              battleState;

    public List<PlayerCharacter>    allySide;
    public List<PlayerCharacter>    enemySide;

    public bool                     inCombat;

    public List<PlayerCharacter>    teamOrder = new List<PlayerCharacter>();
    int                             currentTeamOrderIndex = -1;
    public PlayerCharacter          currentTurn;

    public static BattleLog         battleLog;
    public static CharacterUIList   charUIList;
    public static CharacterActions  charActions;

    private void Start()
    {
        charUIList = FindObjectOfType<CharacterUIList>();
        battleLog = FindObjectOfType<BattleLog>();
        charActions = FindObjectOfType<CharacterActions>();

        allySide = new List<PlayerCharacter>(){ DBPlayerCharacter.GetPC(13) };
        enemySide = new List<PlayerCharacter>() { DBPlayerCharacter.GetPC(1) };

        for (int i = 0; i < allySide.Count; i++)
        {
            teamOrder.Add(allySide[i]);
            allySide[i].team = Character.Team.Ally;
        }

        for (int i = 0; i < enemySide.Count; i++)
        {
            teamOrder.Add(enemySide[i]);
            allySide[i].team = Character.Team.Enemy;
        }

        StartCombat();
    }

    public void                     StartCombat()
    {
        SetBattleState(BattleState.Start);
        StartCoroutine(SetupBattle());
    }

    IEnumerator                     SetupBattle()
    {
        for (int i = 0; i < allySide.Count; i++)
        {
            charUIList.AddChar(allySide[i]);
        }

        for (int i = 0; i < enemySide.Count; i++)
        {
            charUIList.AddChar(enemySide[i]);
        }

        yield return new WaitForSeconds(3);

        SetBattleState(BattleState.AllyPhase);
        currentTurn = teamOrder[0];
        EndTurn();
    }

    public static void              UpdateUIs()
    {
        for (int i = 0; i < charUIList.charList.Count; i++)
        {
            charUIList.charList[i].UpdateUI();
        }
    }

    public void                     SetBattleState      (BattleState state)
    {
        switch (state)
        {
            case BattleState.Start:
                battleState = BattleState.Start;
                charActions.InteractableButtons(false);
                battleLog.LogBattleStatus("COMBAT START!");
                break;

            case BattleState.AllyPhase:
                battleState = BattleState.AllyPhase;
                charActions.InteractableButtons(true);
                battleLog.LogBattleStatus("Ally Phase");
                break;

            case BattleState.EnemyPhase:
                battleState = BattleState.EnemyPhase;
                charActions.InteractableButtons(false);
                battleLog.LogBattleStatus("Enemy Phase");
                break;

            case BattleState.Won:
                charActions.InteractableButtons(false);
                battleLog.LogBattleStatus("Ally team wins!");
                break;
            case BattleState.Lost:
                charActions.InteractableButtons(false);
                battleLog.LogBattleStatus("Enemy team wins!");
                break;
        }
    }

    public void                     EndTurn()
    {
        if(currentTeamOrderIndex + 1 == teamOrder.Count)
        {
            currentTeamOrderIndex = 0;
        }
        else
        {
            currentTeamOrderIndex++;
        }

        bool allyDeath = CheckTotalTeamKill(Character.Team.Ally);
        bool enemyDeath = CheckTotalTeamKill(Character.Team.Enemy);

        if(allyDeath)
        {
            SetBattleState(BattleState.Lost);
        }
        else if (enemyDeath)
        {
            SetBattleState(BattleState.Won);
        }
        else
        {
            currentTurn = teamOrder[currentTeamOrderIndex];
            battleLog.LogBattleStatus($"{currentTurn.name}'s Turn");
            charUIList.CurrentSelection(currentTurn);
        }
    }

    public bool                     CheckTotalTeamKill  (Character.Team team)
    {
        int totalHP = 0;
        bool isDead = false;
        List<PlayerCharacter> teamList = new List<PlayerCharacter>();

        switch (team)
        {
            case Character.Team.Enemy:
                teamList = enemySide;
                break;
            case Character.Team.Ally:
                teamList = allySide;
                break;
            case Character.Team.OutOfCombat:
                break;
            default:
                break;
        }

        for (int i = 0; i < teamList.Count; i++)
        {
            totalHP += teamList[i].stats[Character.Stats.CURHP];
        }

        if(totalHP == 0)
        {
            isDead = true;
        }

        return isDead;
    }
}
