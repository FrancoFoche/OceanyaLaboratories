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
    public static   Character           caster;
    public static   List<Character>     target { get; private set; }

    public          BattleState         battleState;
    public          TurnState           turnState;
    public          bool                inCombat;

    public static   BattleLog           battleLog;
    public static   AllyUIList          charUIList;
    public static   UICharacterActions  charActions;
    public static   BattleManager       instance;

    public          RawImage            easteregg;

    private void Start()
    {
        easteregg.gameObject.SetActive(false);
        charUIList = FindObjectOfType<AllyUIList>();
        battleLog = FindObjectOfType<BattleLog>();
        charActions = FindObjectOfType<UICharacterActions>();
        target = new List<Character>();
        caster = new Character();
        instance = this;

        TeamOrderManager.BuildTeamOrder();
        StartCombat();
    }

    private void Update()
    {
        if (turnState == TurnState.WaitingForTarget)
        {
            if (Input.GetKeyDown(KeyCode.Return) || target.Count == UICharacterActions.instance.maxTargets)
            {
                UICharacterActions.instance.Act(caster, target);
                SetTurnState(TurnState.WaitingForAction);
            }
            else
            {
                target = charUIList.CheckTargets();
            }
        }
        else
        {
            if (charUIList.toggleGroup.AnyTogglesOn() && charUIList.different)
            {
                GetCaster();
                UISkillContext.instance.Hide();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            battleLog.LogBattleEffect("The GM decided to revert back to the turn that was supposed to take place. Smh.");
            ReselectOriginalTurn();
        }
    }


    public void                     StartCombat         ()                  
    {
        SetBattleState(BattleState.Start);
        StartCoroutine(SetupBattle());
    }
    public IEnumerator              SetupBattle         ()                  
    {
        for (int i = 0; i < TeamOrderManager.allySide.Count; i++)
        {
            charUIList.AddChar(TeamOrderManager.allySide[i]);
        }

        for (int i = 0; i < TeamOrderManager.enemySide.Count; i++)
        {
            charUIList.AddChar(TeamOrderManager.enemySide[i]);
        }

        for (int i = 0; i < TeamOrderManager.teamOrder.Count; i++)
        {
            TeamOrderManager.teamOrder[i].CheckPassives();
            TeamOrderManager.teamOrder[i].ActivatePassiveEffects(PassiveActivation.StartOfBattle);
        }

        yield return new WaitForSeconds(3);

        SetBattleState(BattleState.AllyPhase);
        TeamOrderManager.SetCurrentTurn(TeamOrderManager.teamOrder[0]);
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
    public void                     SetTurnState        (TurnState state)   
    {
        switch (state)
        {
            case TurnState.WaitingForAction:
                UICharacterActions.instance.InteractableButtons(true);
                UISkillContext.instance.InteractableButtons(true);
                charUIList.TurnToggleGroup(true);
                turnState = TurnState.WaitingForAction;
                break;


            case TurnState.WaitingForTarget:
                UICharacterActions.instance.InteractableButtons(false);
                UISkillContext.instance.InteractableButtons(false);
                charUIList.TurnToggleGroup(false);
                charUIList.TurnToggles(false);

                turnState = TurnState.WaitingForTarget;
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
    public void                     GetCaster           ()                  
    {
        if (TeamOrderManager.currentTurn != AllyUIList.curCharacterSelected)
        {
            caster = AllyUIList.curCharacterSelected;
            battleLog.LogBattleStatus($"{caster.name}'s (Non-Ordered) Turn");
        }
        else
        {
            caster = TeamOrderManager.currentTurn;
        }
    }
    public void                     ClearTargets        ()                  
    {
        target = new List<Character>();
    }
    public void                     ResetCheckedPassives()                  
    {
        caster.checkedPassives = false;

        for (int i = 0; i < target.Count; i++)
        {
            target[i].checkedPassives = false;
        }
    }
    public void                     ReselectOriginalTurn()                  
    {
        charUIList.SelectCharacter(TeamOrderManager.currentTurn);
        battleLog.LogBattleStatus($"{TeamOrderManager.currentTurn.name}'s Turn. Again.");
        SetTurnState(TurnState.WaitingForAction);
    }
    
}
