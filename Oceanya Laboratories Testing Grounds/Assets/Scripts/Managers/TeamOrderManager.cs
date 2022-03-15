using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnState
{
    Start,
    WaitingForAction,
    WaitingForTarget,
    WaitingForConfirmation,
    End,
    NonDefined
}


/// <summary>
/// Where everything about turn order, and turn state is located
/// </summary>
public static class TeamOrderManager
{
    public  static  List<Character>     allySide                        { get; private set; }
    public  static  List<Character>     enemySide                       { get; private set; }
    public  static  List<Character>     totalCharList                   { get; private set; }
    public  static  TurnState           turnState                       { get; set; }
    public  static  bool                AIturn                          { get; private set; }
    public  static  int                 currentTeamOrderIndex           { get; private set; }
    public  static  Character           currentTurn;
    public  static  SPDSystem           spdSystem;

    public static void                  BuildTeamOrder  (Wave battle)                                           
    {
        allySide = battle.allySide;
        enemySide = battle.enemySide;
        
        totalCharList = new List<Character>();
        for (int i = 0; i < allySide.Count; i++)
        {
            totalCharList.Add(allySide[i]);
            allySide[i].SetTeam(Team.Ally);
            allySide[i].SetAIControlled(false);

            if(BattleManager.i.battleState == BattleState.Won)
            {
                allySide[i].ResetToOriginalStatBuffs();
            }
            else
            {
                allySide[i].ResetFull();
            }
        }
        for (int i = 0; i < enemySide.Count; i++)
        {
            totalCharList.Add(enemySide[i]);
            enemySide[i].SetTeam(Team.Enemy);
            enemySide[i].SetAIControlled(true);
            enemySide[i].ResetFull();
        }

        turnState = TurnState.NonDefined;

        currentTeamOrderIndex = 0;
        currentTurn = new Character();

        BuildSPDSystem(allySide, enemySide);
    }
    public static void                  BuildSPDSystem  (List<Character> allySide, List<Character> enemySide)   
    {
        spdSystem = new SPDSystem(allySide, enemySide);
        spdSystem.BuildSPDSystem();

        BattleManager.i.teamOrderMenu.LoadTeamOrder(spdSystem.teamOrder, currentTeamOrderIndex);
    }
    public static void                  SetCurrentTurn  (Character character)                                   
    {
        currentTurn = character;

        BattleManager.i.battleLog.LogTurn(currentTurn);
        BattleManager.i.uiList.SelectCharacter(currentTurn);
    }
    public static void                  SetTurnState    (TurnState state)                                       
    {
        if (turnState == state && state != TurnState.NonDefined)
        {
            Debug.LogWarning("Turn state is ALREADY set to " + state.ToString() + ".");
        }
        else
        {
            Character caster = BattleManager.caster;
            List<Character> target = BattleManager.target;

            switch (state)
            {
                case TurnState.Start:
                    {
                        turnState = TurnState.Start;
                        UISkillContext.instance.Hide();
                        UIItemContext.instance.Hide();
                        if (caster.defending)
                        {
                            caster.SetDefending(false);
                        }

                        if (!caster.dead)
                        {
                            caster.ActivatePassiveEffects(ActivationTime_General.StartOfTurn);

                            if (caster.dead)
                            {
                                BattleManager.i.battleLog.LogBattleEffect("Aaaaaand they died. Oh well, next turn.");
                                EndTurn();
                                return;
                            }
                        }

                        if (!caster.AIcontrolled || SettingsManager.manualMode)
                        {
                            AIturn = false;
                            SetTurnState(TurnState.WaitingForAction);
                        }
                        else
                        {
                            AIturn = true;
                            UICharacterActions.instance.InteractableButtons(false);
                            UISkillContext.instance.InteractableButtons(false);
                            UIItemContext.instance.InteractableButtons(false);
                            BattleManager.i.uiList.InteractableUIs(false);
                            BattleManager.i.uiList.SetTargettingMode(false);

                            BattleManager.i.DelayAction(3, caster.AITurn);
                        }
                    }
                    break;

                case TurnState.WaitingForAction:
                    {
                        UICharacterActions.instance.InteractableButtons(true);
                        UISkillContext.instance.InteractableButtons(true);
                        UIItemContext.instance.InteractableButtons(true);
                        BattleManager.i.uiList.TurnToggleGroup(true);
                        BattleManager.i.uiList.SetTargettingMode(false);

                        if(UISkillContext.instance.gameObject.activeSelf)
                        {
                            UISkillContext.instance.DeactivateVisualSelect();
                        }
                        else if (UIItemContext.instance.gameObject.activeSelf)
                        {
                            UIItemContext.instance.DeactivateVisualSelect();
                        }
                        else
                        {
                            UICharacterActions.instance.DeactivateVisualSelect();
                        }

                        if (SettingsManager.manualMode)
                        {
                            BattleManager.i.uiList.InteractableUIs(true);
                        }
                        else
                        {
                            BattleManager.i.uiList.InteractableUIs(false);
                        }

                        turnState = TurnState.WaitingForAction;
                    }
                    break;

                case TurnState.WaitingForTarget:
                    {
                        UICharacterActions.instance.InteractableButtons(false);
                        UISkillContext.instance.InteractableButtons(false);
                        UIItemContext.instance.InteractableButtons(false);
                        BattleManager.i.uiList.TurnToggleGroup(false);
                        BattleManager.i.uiList.InteractableUIs(true);
                        BattleManager.i.uiList.TurnToggles(false);

                        if (!AIturn)
                        {
                            BattleManager.i.battleLog.LogBattleEffect($"Choose a target! (Use Space to confirm targets midway.)");
                            BattleManager.i.uiList.SetTargettingMode(true);
                        }

                        BattleManager.i.ClearTargets();
                        turnState = TurnState.WaitingForTarget;
                    }
                    break;

                case TurnState.WaitingForConfirmation:
                    {
                        System.Action action = delegate { UICharacterActions.instance.Act(new ActionData(caster, target)); };
                        if (AIturn)
                        {
                            //Activate it immediately
                            action();
                        }
                        else
                        {
                            UICharacterActions.instance.StartButtonActionConfirmation(action);
                        }
                    }
                    break;

                case TurnState.End:
                    {
                        turnState = TurnState.End;

                        if (!caster.dead)
                        {
                            caster.ActivatePassiveEffects(ActivationTime_General.EndOfTurn);
                        }
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Continues through the team order list, setting currentTurn to the next ordered turn.
    /// </summary>
    public static void                  Continue        ()                      
    {
        if (currentTeamOrderIndex + 1 == spdSystem.teamOrder.Count)
        {
            spdSystem.SetNextPeriod();
            BattleManager.i.battleLog.LogBattleStatus("TOP OF THE ROUND");
            currentTeamOrderIndex = 0;
        }
        else
        {
            currentTeamOrderIndex++;
        }

        Character newTurn = spdSystem.teamOrder[currentTeamOrderIndex];
        SetCurrentTurn(newTurn);
        BattleManager.i.DelayAction(0, () => BattleManager.i.uiList.SelectCharacter(currentTurn));

        if (currentTurn.dead)
        {
            BattleManager.i.battleLog.LogBattleEffect($"But {currentTurn.name} was already dead... F.");
            Continue();
            return;
        }
    }
    

    public static void                  EndTurn         ()                      
    {
        SetTurnState(TurnState.End);

        BattleManager.i.TotalTeamKill_Check();

        if (BattleManager.i.inCombat)
        {
            if (BattleManager.caster == currentTurn)
            {
                currentTurn.SetPlayed();
                currentTurn.UpdateCDs();
                Continue();
            }
            else
            {
                BattleManager.caster.SetPlayed();
                BattleManager.caster.UpdateCDs();
                BattleManager.i.ReselectOriginalTurn();
            }

            BattleManager.i.GetCaster();
            BattleManager.i.teamOrderMenu.UpdateTeamOrder(currentTeamOrderIndex);
            SetTurnState(TurnState.Start);
        }
    }
}

public class SPDSystem
{
    List<Character>[] teams;
    public int                  GensPerPeriod   { get; private set; }
    public int                  MaxDelay        { get; private set; }
    public int                  CurrentPeriod   { get; private set; }
    public int                  CurrentGen      { get; private set; }
    public List<SPDSystemInfo>  info            { get; private set; }
    public List<Character>      teamOrder       { get; private set; }

    public SPDSystem                        (params List<Character>[] teams)            
    {
        GensPerPeriod = 15;
        CurrentPeriod = 0;
        this.teams = teams;
        UpdateMaxDelay(this.teams);
    }

    public void         BuildSPDSystem      ()                                          
    {
        info = new List<SPDSystemInfo>();

        for (int i = 0; i < teams.Length; i++)
        {
            List<Character> curTeam = teams[i];

            for (int j = 0; j < curTeam.Count; j++)
            {
                Character curCharacter = curTeam[j];
                SPDSystemInfo systemInfo = new SPDSystemInfo(curCharacter);

                info.Add(systemInfo);
            }
        }

        SetNextPeriod();
    }
    void                UpdateMaxDelay      (params List<Character>[] teams)            
    {
        int maxDelay = 0;

        for (int i = 0; i < teams.Length; i++)
        {
            List<Character> curTeam = teams[i];
            int maxAGIteam = 0;

            for (int j = 0; j < curTeam.Count; j++)
            {
                Character curCharacter = curTeam[j];
                int curAGI = curCharacter.GetStat(Stats.AGI).value;

                if (curAGI > maxAGIteam)
                {
                    maxAGIteam = curAGI;
                }
            }

            maxDelay += maxAGIteam;
        }

        MaxDelay = maxDelay;
    }
    public void         SetNextPeriod       ()                                          
    {
        SetCurrentPeriod(CurrentPeriod + 1);
    }
    void                SetCurrentPeriod    (int period)                                
    {
        CurrentPeriod = period;
        UpdateCurrentGen();
        BuildTeamOrder(info);
    }
    void                UpdateCurrentGen    ()                                          
    {
        CurrentGen = 1+((CurrentPeriod-1) * GensPerPeriod);
    }
    void                BuildTeamOrder      (List<SPDSystemInfo> info)                  
    {
        List<Character> newteamOrder = new List<Character>();

        for (int i = 0; i < GensPerPeriod; i++)
        {
            for (int j = 0; j < info.Count; j++)
            {
                if (info[j].turnList[i])
                {
                    newteamOrder.Add(info[j].character);
                }
            }
        }

        teamOrder = newteamOrder;
    }

    /// <summary>
    /// Swaps two characters in team order, returns success.
    /// </summary>
    /// <param name="character"></param>
    /// <param name="swapWith"></param>
    /// <returns></returns>
    public bool         Swap                (Character character, Character swapWith)   
    {
        int index1 = teamOrder.IndexOf(character, TeamOrderManager.currentTeamOrderIndex);
        int swapIndex = teamOrder.IndexOf(swapWith, TeamOrderManager.currentTeamOrderIndex);

        if (index1 != -1 && swapIndex != -1)
        {
            Character temp = teamOrder[swapIndex];

            teamOrder[swapIndex] = teamOrder[index1];
            teamOrder[index1] = temp;
            return true;
        }

        return false;
    }
}

public class SPDSystemInfo
{
    public Character    character       { get; private set; }
    public int          AGI             { get; private set; }
    public float        CounterIncrease { get; private set; }
    public float        CurrentCounter  { get; private set; }
    public List<bool>   turnList        { get; private set; }

    public SPDSystemInfo                (Character character)   
    {
        this.character = character;
        AGI = character.GetStat(Stats.AGI).value;
        int maxDelay = TeamOrderManager.spdSystem.MaxDelay;
        CounterIncrease = (float)(AGI + (maxDelay * 0.2)) / maxDelay;
        UpdateCurrentCounter();
        GenerateTurns();
    }

    public void UpdateCurrentCounter    ()                      
    {
        CurrentCounter = TeamOrderManager.spdSystem.CurrentGen * CounterIncrease;
    }
    public void GenerateTurns           ()                      
    {
        turnList = new List<bool>();
        float pastTotalCounter = 0;
        for (int i = 0; i < TeamOrderManager.spdSystem.GensPerPeriod; i++)
        {
            float curTotalCounter = CurrentCounter + (CounterIncrease * i);

            bool turn = false;

            if (i == 0)
            {
                turn = Mathf.Floor(CurrentCounter) == Mathf.Floor(curTotalCounter) ? false : true;
            }
            else
            {
                turn = Mathf.Floor(pastTotalCounter) == Mathf.Floor(curTotalCounter) ? false : true;
            }

            turnList.Add(turn);
            pastTotalCounter = curTotalCounter;
        }
    }
}
