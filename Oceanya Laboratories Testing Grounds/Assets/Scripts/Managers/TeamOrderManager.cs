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
    public  static  TurnState           turnState                       { get; private set; }
    public  static  bool                AIturn                          { get; private set; }
    public  static  int                 currentTeamOrderIndex           { get; private set; }
    public  static  Character           currentTurn;
    public  static  SPDSystem           spdSystem;

    public static void                  BuildTeamOrder  ()                      
    {
        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9) };
        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(1), GameAssetsManager.instance.GetEnemy(2), GameAssetsManager.instance.GetEnemy(3) }; //Ally side and enemy side should be set outside of this script, this is here for testing reasons
        
        totalCharList = new List<Character>();
        for (int i = 0; i < allySide.Count; i++)
        {
            totalCharList.Add(allySide[i]);
            allySide[i].SetTeam(Team.Ally);
            allySide[i].SetAIControlled(false);
        }
        for (int i = 0; i < enemySide.Count; i++)
        {
            totalCharList.Add(enemySide[i]);
            enemySide[i].SetTeam(Team.Enemy);
            enemySide[i].SetAIControlled(true);
        }

        turnState = TurnState.NonDefined;

        currentTeamOrderIndex = 0;
        currentTurn = new Character();

        spdSystem = new SPDSystem(allySide, enemySide);
        spdSystem.BuildSPDSystem();
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

                        if (caster.defending)
                        {
                            caster.SetDefending(false);
                        }

                        if (!caster.dead)
                        {
                            caster.ActivatePassiveEffects(ActivationTime.StartOfTurn);
                        }

                        if (!caster.AIcontrolled || BattleManager.i.debugMode)
                        {
                            AIturn = false;
                            SetTurnState(TurnState.WaitingForAction);
                        }
                        else
                        {
                            AIturn = true;
                            UICharacterActions.instance.InteractableButtons(false);
                            UISkillContext.instance.InteractableButtons(false);
                            BattleManager.i.uiList.InteractableToggles(false);

                            BattleManager.i.DelayAction(3, caster.AITurn);
                        }
                    }
                    break;

                case TurnState.WaitingForAction:
                    {
                        UICharacterActions.instance.InteractableButtons(true);
                        UISkillContext.instance.InteractableButtons(true);
                        BattleManager.i.uiList.TurnToggleGroup(true);

                        if (BattleManager.i.debugMode)
                        {
                            BattleManager.i.uiList.InteractableToggles(true);
                        }
                        else
                        {
                            BattleManager.i.uiList.InteractableToggles(false);
                        }

                        BattleManager.i.uiList.SetTargettingMode(false);

                        turnState = TurnState.WaitingForAction;
                    }
                    break;

                case TurnState.WaitingForTarget:
                    {
                        UICharacterActions.instance.InteractableButtons(false);
                        UISkillContext.instance.InteractableButtons(false);
                        BattleManager.i.uiList.TurnToggleGroup(false);
                        BattleManager.i.uiList.TurnToggles(false);
                        BattleManager.i.uiList.InteractableToggles(true);
                        BattleManager.i.uiList.SetTargettingMode(true);

                        BattleManager.i.ClearTargets();
                        turnState = TurnState.WaitingForTarget;
                    }
                    break;

                case TurnState.WaitingForConfirmation:
                    {
                        turnState = TurnState.WaitingForConfirmation;
                        if (AIturn)
                        {
                            UICharacterActions.instance.InstantConfirmAction(caster, target, UICharacterActions.instance.Act);
                        }
                        else
                        {
                            UICharacterActions.instance.DelayConfirmAction(caster, target, UICharacterActions.instance.Act);
                        }

                    }
                    break;

                case TurnState.End:
                    {
                        turnState = TurnState.End;

                        if (!caster.dead)
                        {
                            caster.ActivatePassiveEffects(ActivationTime.EndOfTurn);
                        }

                        BattleManager.i.CheckTotalTeamKill();
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
                int curAGI = curCharacter.stats[Stats.AGI];

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
    public void         Swap                (Character character, Character swapWith)   
    {
        int index1 = teamOrder.IndexOf(character, TeamOrderManager.currentTeamOrderIndex);
        int swapIndex = teamOrder.IndexOf(swapWith, TeamOrderManager.currentTeamOrderIndex);

        Character temp = teamOrder[swapIndex];

        teamOrder[swapIndex] = teamOrder[index1];
        teamOrder[index1] = temp;
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
        AGI = character.stats[Stats.AGI];
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
