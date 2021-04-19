﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TeamOrderManager
{
    public  static  List<Character>     allySide;
    public  static  List<Character>     enemySide;
    public  static  List<Character>     teamOrder               = new List<Character>();
    public  static  int                 currentTeamOrderIndex   = -1;
    public  static  Character           currentTurn;
    public  static  int                 phaseChangeIndex;

    /// <summary>
    /// Checks for when its enemy phase and ally phase. This should not be this way in the final game, but with a simpler system like this it works well enough.
    /// </summary>
    public static BattleState           CheckPhase()        
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
    public static void                  BuildTeamOrder()    {

        allySide = new List<Character>() { DBPlayerCharacter.GetPC(13), DBPlayerCharacter.GetPC(10), DBPlayerCharacter.GetPC(11), DBPlayerCharacter.GetPC(40) };
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
    public static IEnumerator           SetupBattle()       
    {
        for (int i = 0; i < allySide.Count; i++)
        {
            BattleManager.charUIList.AddChar(allySide[i]);
        }

        for (int i = 0; i < enemySide.Count; i++)
        {
            BattleManager.charUIList.AddChar(enemySide[i]);
        }

        for (int i = 0; i < teamOrder.Count; i++)
        {
            teamOrder[i].CheckPassives();
            teamOrder[i].ActivatePassiveEffects(PassiveActivation.StartOfBattle);
        }

        yield return new WaitForSeconds(3);

        BattleManager.instance.SetBattleState(BattleState.AllyPhase);
        currentTurn = teamOrder[0];
        EndTurn();
    }
    public static void                  EndTurn()           
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
            BattleState checkPhase = CheckPhase();
            if (checkPhase != BattleManager.instance.battleState)
            {
                BattleManager.instance.SetBattleState(checkPhase);
            }

            currentTurn = teamOrder[currentTeamOrderIndex];
            
            BattleManager.battleLog.LogBattleStatus($"{currentTurn.name}'s Turn");
            BattleManager.charUIList.SelectCharacter(currentTurn);

            currentTurn.timesPlayed += 1;
            Debug.Log($"currentTurn = {currentTurn.name}; Times played = {currentTurn.timesPlayed}");

            if (currentTurn.dead)
            {
                BattleManager.battleLog.LogBattleEffect($"But {currentTurn.name} was already dead... F.");
                EndTurn();
            }
        }
    }
}
