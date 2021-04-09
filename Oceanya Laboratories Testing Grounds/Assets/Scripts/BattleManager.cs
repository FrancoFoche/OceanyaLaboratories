using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    WaitingForTarget
}

public class BattleManager : MonoBehaviour
{
    public          BattleState         battleState;
    public          TurnState           turnState;
    public          bool                inCombat;

    public static   BattleLog           battleLog;
    public static   CharacterUIList     charUIList;
    public static   CharacterActions    charActions;

    public RawImage easteregg;

    private void Start()
    {
        easteregg.gameObject.SetActive(false);
        charUIList = FindObjectOfType<CharacterUIList>();
        battleLog = FindObjectOfType<BattleLog>();
        charActions = FindObjectOfType<CharacterActions>();

        TeamOrderManager.BuildTeamOrder();
        StartCombat();
    }

    public void                     StartCombat()
    {
        SetBattleState(BattleState.Start);
        StartCoroutine(SetupBattle());
    }

    IEnumerator                     SetupBattle()
    {
        for (int i = 0; i < TeamOrderManager.allySide.Count; i++)
        {
            charUIList.AddChar(TeamOrderManager.allySide[i]);
        }

        for (int i = 0; i < TeamOrderManager.enemySide.Count; i++)
        {
            charUIList.AddChar(TeamOrderManager.enemySide[i]);
        }

        yield return new WaitForSeconds(3);

        SetBattleState(BattleState.AllyPhase);
        TeamOrderManager.currentTurn = TeamOrderManager.teamOrder[0];
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
                easteregg.gameObject.SetActive(true);
                break;
            case BattleState.Lost:
                charActions.InteractableButtons(false);
                battleLog.LogBattleStatus("Enemy team wins!");
                break;
        }
    }

    public void                     EndTurn()
    {
        if(TeamOrderManager.currentTeamOrderIndex + 1 == TeamOrderManager.teamOrder.Count)
        {
            battleLog.LogBattleStatus("TOP OF THE ROUND");
            TeamOrderManager.currentTeamOrderIndex = 0;
        }
        else
        {
            TeamOrderManager.currentTeamOrderIndex++;
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
            BattleState checkPhase = TeamOrderManager.CheckPhase();
            if (checkPhase != battleState)
            {
                SetBattleState(checkPhase);
            }

            TeamOrderManager.currentTurn = TeamOrderManager.teamOrder[TeamOrderManager.currentTeamOrderIndex];
            battleLog.LogBattleStatus($"{TeamOrderManager.currentTurn.name}'s Turn");
            charUIList.SelectCharacter(TeamOrderManager.currentTurn);

            if (TeamOrderManager.currentTurn.dead)
            {
                battleLog.LogBattleEffect($"But {TeamOrderManager.currentTurn.name} was already dead... F.");
                EndTurn();
            }
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
                teamList = TeamOrderManager.enemySide;
                break;
            case Character.Team.Ally:
                teamList = TeamOrderManager.allySide;
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
