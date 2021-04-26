using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState
{
    Start,
    Won,
    Lost
}

public class BattleManager : MonoBehaviour
{
    public static   Character           caster;
    public static   List<Character>     target { get; private set; }

    public          BattleState         battleState;
    
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
        if (TeamOrderManager.turnState == TurnState.WaitingForTarget)
        {
            if (Input.GetKeyDown(KeyCode.Return) || target.Count == UICharacterActions.instance.maxTargets)
            {
                Debug.Log("Targetting done.");
                BattleManager.charUIList.TurnToggles(false);
                UICharacterActions.instance.Act(caster, target);
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
        charActions.AddAllActions();
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

        for (int i = 0; i < TeamOrderManager.totalCharList.Count; i++)
        {
            TeamOrderManager.totalCharList[i].CheckPassives();
            TeamOrderManager.totalCharList[i].ActivatePassiveEffects(ActivationTime.StartOfBattle);
        }

        yield return new WaitForSeconds(3);

        caster = TeamOrderManager.spdSystem.teamOrder[0];
        TeamOrderManager.SetCurrentTurn(caster);
        caster.ActivatePassiveEffects(ActivationTime.StartOfTurn);
        TeamOrderManager.SetTurnState(TurnState.WaitingForAction);
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
    public void                     GetCaster           ()                  
    {
        if (TeamOrderManager.currentTurn != AllyUIList.curCharacterSelected)
        {
            caster = AllyUIList.curCharacterSelected;
            battleLog.LogTurn(caster, 3);
            caster.ActivatePassiveEffects(ActivationTime.StartOfTurn);
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
        battleLog.LogTurn(TeamOrderManager.currentTurn, 2);
    }
    
}
