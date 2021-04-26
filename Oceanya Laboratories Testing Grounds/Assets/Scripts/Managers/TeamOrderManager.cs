using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnState
{
    Start,
    WaitingForAction,
    WaitingForTarget,
    End
}


/// <summary>
/// Where everything about turn order, and turn state is located
/// </summary>
public static class TeamOrderManager
{
    public  static  List<Character>     allySide;
    public  static  List<Character>     enemySide;
    public  static  List<Character>     totalCharList                   { get; private set; }
    public  static  TurnState           turnState                       { get; private set; }
    public  static  int                 currentTeamOrderIndex   = 0;
    public  static  Character           currentTurn;
    public  static  SPDSystem           spdSystem;

    public static void                  BuildTeamOrder  ()                      {
        totalCharList = new List<Character>();

        allySide = new List<Character>() { DBPlayerCharacter.GetPC(0), DBPlayerCharacter.GetPC(13), DBPlayerCharacter.GetPC(10), DBPlayerCharacter.GetPC(11), DBPlayerCharacter.GetPC(40) };
        enemySide = new List<Character>() { DBPlayerCharacter.GetPC(1), DBPlayerCharacter.GetPC(17), DBPlayerCharacter.GetPC(31), DBPlayerCharacter.GetPC(420), DBPlayerCharacter.GetPC(666) }; //Ally side and enemy side should be set outside of this script, this is here for testing reasons


        for (int i = 0; i < allySide.Count; i++)
        {
            totalCharList.Add(allySide[i]);
            allySide[i].team = Team.Ally;
        }

        for (int i = 0; i < enemySide.Count; i++)
        {
            totalCharList.Add(enemySide[i]);
            enemySide[i].team = Team.Enemy;
        }

        spdSystem = new SPDSystem(allySide, enemySide);
        spdSystem.BuildSPDSystem();
    }
    public static void                  SetCurrentTurn  (Character character)   
    {
        currentTurn = character;

        BattleManager.battleLog.LogTurn(currentTurn);
        BattleManager.charUIList.SelectCharacter(currentTurn);
    }
    public static void                  SetTurnState    (TurnState state)       
    {
        switch (state)
        {
            case TurnState.Start:
                turnState = TurnState.Start;

                //function that tells the character that its the start of the turn (activate all skills that require it)
                BattleManager.caster.ActivatePassiveEffects(ActivationTime.StartOfTurn);

                SetTurnState(TurnState.WaitingForAction);
                break;
            case TurnState.WaitingForAction:
                UICharacterActions.instance.InteractableButtons(true);
                UISkillContext.instance.InteractableButtons(true);
                BattleManager.charUIList.TurnToggleGroup(true);
                turnState = TurnState.WaitingForAction;
                break;


            case TurnState.WaitingForTarget:
                UICharacterActions.instance.InteractableButtons(false);
                UISkillContext.instance.InteractableButtons(false);
                BattleManager.charUIList.TurnToggleGroup(false);
                BattleManager.charUIList.TurnToggles(false);

                turnState = TurnState.WaitingForTarget;
                break;

            case TurnState.End:
                turnState = TurnState.End;
                BattleManager.caster.ActivatePassiveEffects(ActivationTime.EndOfTurn);
                break;
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
            BattleManager.battleLog.LogBattleStatus("TOP OF THE ROUND");
            currentTeamOrderIndex = 0;
        }
        else
        {
            currentTeamOrderIndex++;
        }

        SetCurrentTurn(spdSystem.teamOrder[currentTeamOrderIndex]);
    }
    public static void                  EndTurn         ()                      
    {
        SetTurnState(TurnState.End);

        bool allyDeath = BattleManager.instance.CheckTotalTeamKill(Team.Ally);
        bool enemyDeath = BattleManager.instance.CheckTotalTeamKill(Team.Enemy);

        if (allyDeath)
        {
            BattleManager.instance.SetBattleState(BattleState.Lost);
        }
        else if (enemyDeath)
        {
            BattleManager.instance.SetBattleState(BattleState.Won);
        }
        else
        {
            if (BattleManager.caster == currentTurn)
            {
                currentTurn.timesPlayed += !currentTurn.dead ? 1 : 0;
                currentTurn.UpdateCDs();
                Continue();
            }
            else
            {
                BattleManager.caster.timesPlayed += 1;
                BattleManager.caster.UpdateCDs();
                BattleManager.instance.ReselectOriginalTurn();
            }

            BattleManager.instance.GetCaster();

            SetTurnState(TurnState.Start);

            SetTurnState(TurnState.WaitingForAction);

            if (currentTurn.dead)
            {
                BattleManager.battleLog.LogBattleEffect($"But {currentTurn.name} was already dead... F.");
                EndTurn();
            }
        }
    }
}

public class SPDSystem
{
    List<Character>[] teams;
    public int GensPerPeriod { get; private set; }
    public int MaxDelay { get; private set; }
    public int CurrentPeriod { get; private set; }
    public int CurrentGen { get; private set; }
    List<SPDSystemInfo> info = new List<SPDSystemInfo>();
    public List<Character> teamOrder { get; private set; }

    public SPDSystem(params List<Character>[] teams)
    {
        GensPerPeriod = 15;
        CurrentPeriod = 0;
        this.teams = teams;
        UpdateMaxDelay(this.teams);
    }

    public void BuildSPDSystem()
    {
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

    void UpdateMaxDelay(params List<Character>[] teams)
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
    public void SetNextPeriod()
    {
        SetCurrentPeriod(CurrentPeriod + 1);
    }

    void SetCurrentPeriod(int period)
    {
        CurrentPeriod = period;
        UpdateCurrentGen();
        BuildTeamOrder(info);
    }

    void UpdateCurrentGen()
    {
        CurrentGen = 1+((CurrentPeriod-1) * GensPerPeriod);
    }

    void BuildTeamOrder(List<SPDSystemInfo> info)
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
}

public class SPDSystemInfo
{
    public Character character { get; private set; }
    public int AGI { get; private set; }
    public float CounterIncrease { get; private set; }
    public float CurrentCounter { get; private set; }
    public List<bool> turnList { get; private set; }

    public SPDSystemInfo(Character character)
    {
        this.character = character;
        AGI = character.stats[Stats.AGI];
        int maxDelay = TeamOrderManager.spdSystem.MaxDelay;
        CounterIncrease = (float)(AGI + (maxDelay * 0.2)) / maxDelay;
        UpdateCurrentCounter();
        GenerateTurns();
    }

    public void UpdateCurrentCounter()
    {
        CurrentCounter = TeamOrderManager.spdSystem.CurrentGen * CounterIncrease;
    }

    public void GenerateTurns()
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
