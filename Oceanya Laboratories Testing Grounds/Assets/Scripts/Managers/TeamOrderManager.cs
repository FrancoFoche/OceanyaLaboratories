﻿using System.Collections;
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
    public  static  List<Character>     teamOrder               = new List<Character>();
    public  static  TurnState           turnState { get; private set; }
    public  static  int                 currentTeamOrderIndex   = 0;
    public  static  Character           currentTurn;
    public  static  int                 phaseChangeIndex;

    /// <summary>
    /// Checks for when its enemy phase and ally phase. This should not be this way in the final game, but with a simpler system like this it works well enough.
    /// </summary>
    public static BattleState           CheckPhase      ()                      
    {
        if(currentTeamOrderIndex >= phaseChangeIndex)
        {
            return BattleState.EnemyPhase;
        }
        else
        {
            return BattleState.AllyPhase;
        }
    }
    public static void                  BuildTeamOrder  ()                      {

        allySide = new List<Character>() { DBPlayerCharacter.GetPC(0), DBPlayerCharacter.GetPC(13), DBPlayerCharacter.GetPC(10), DBPlayerCharacter.GetPC(11), DBPlayerCharacter.GetPC(40) };
        enemySide = new List<Character>() { DBPlayerCharacter.GetPC(1), DBPlayerCharacter.GetPC(17), DBPlayerCharacter.GetPC(31), DBPlayerCharacter.GetPC(420), DBPlayerCharacter.GetPC(666) }; //Ally side and enemy side should be set outside of this script, this is here for testing reasons


        for (int i = 0; i < allySide.Count; i++)
        {
            teamOrder.Add(allySide[i]);
            allySide[i].team = Team.Ally;
        }

        for (int i = 0; i < enemySide.Count; i++)
        {
            teamOrder.Add(enemySide[i]);
            enemySide[i].team = Team.Enemy;
        }

        phaseChangeIndex = allySide.Count;
    }
    public static void                  SetCurrentTurn  (Character character)   
    {
        currentTurn = character;

        BattleManager.battleLog.LogBattleStatus($"{currentTurn.name}'s Turn");
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
                BattleManager.caster.ActivatePassiveEffects(ActivationTime.EndOfTurn);
                turnState = TurnState.End;
                break;
        }
    }

    /// <summary>
    /// Continues through the team order list, setting currentTurn to the next ordered turn.
    /// </summary>
    public static void                  Continue        ()                      
    {
        if (currentTeamOrderIndex + 1 == teamOrder.Count)
        {
            BattleManager.battleLog.LogBattleStatus("TOP OF THE ROUND");
            currentTeamOrderIndex = 0;
        }
        else
        {
            currentTeamOrderIndex++;
        }

        SetCurrentTurn(teamOrder[currentTeamOrderIndex]);
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

            BattleState checkPhase = CheckPhase();
            if (checkPhase != BattleManager.instance.battleState)
            {
                BattleManager.instance.SetBattleState(checkPhase);
            }

            SetTurnState(TurnState.Start);

            if (currentTurn.dead)
            {
                BattleManager.battleLog.LogBattleEffect($"But {currentTurn.name} was already dead... F.");
                EndTurn();
            }
        }
    }
}
