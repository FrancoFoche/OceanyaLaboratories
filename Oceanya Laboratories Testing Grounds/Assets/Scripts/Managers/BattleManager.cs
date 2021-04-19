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
    public static   AllyUIList          charUIList;
    public static   CharacterActions    charActions;
    public static   BattleManager       instance;

    public RawImage easteregg;

    private void Start()
    {
        easteregg.gameObject.SetActive(false);
        charUIList = FindObjectOfType<AllyUIList>();
        battleLog = FindObjectOfType<BattleLog>();
        charActions = FindObjectOfType<CharacterActions>();
        instance = this;

        TeamOrderManager.BuildTeamOrder();
        StartCombat();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            battleLog.LogBattleEffect("The GM decided to revert back to the turn that was supposed to take place. Smh.");
            charUIList.SelectCharacter(TeamOrderManager.currentTurn);
            battleLog.LogBattleStatus($"{TeamOrderManager.currentTurn.name}'s Turn. Again.");
        }
    }


    public void                     StartCombat         ()                  
    {
        SetBattleState(BattleState.Start);
        StartCoroutine(TeamOrderManager.SetupBattle());
    }
    public static void              UpdateUIs           ()                  
    {
        for (int i = 0; i < charUIList.list.Count; i++)
        {
            charUIList.list[i].GetComponent<BattleUI>().UpdateUI();
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
    public bool                     CheckTotalTeamKill  (Team team)         
    {
        int totalHP = 0;
        bool isDead = false;
        List<Character> teamList = new List<Character>();

        switch (team)
        {
            case Team.Enemy:
                teamList = TeamOrderManager.enemySide;
                break;
            case Team.Ally:
                teamList = TeamOrderManager.allySide;
                break;
            case Team.OutOfCombat:
                break;
            default:
                break;
        }

        for (int i = 0; i < teamList.Count; i++)
        {
            totalHP += teamList[i].stats[Stats.CURHP];
        }

        if(totalHP == 0)
        {
            isDead = true;
        }

        return isDead;
    }
}
