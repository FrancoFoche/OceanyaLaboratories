using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TeamOrderManager
{
    public  static  List<PlayerCharacter>   allySide;
    public  static  List<PlayerCharacter>   enemySide;
    public  static  List<PlayerCharacter>   teamOrder               = new List<PlayerCharacter>();
    public  static  int                     currentTeamOrderIndex   = -1;
    public  static  PlayerCharacter         currentTurn;
    public  static  int                     phaseChangeIndex;

    /// <summary>
    /// Checks for when its enemy phase and ally phase. This should not be this way in the final game, but with a simpler system like this it works well enough.
    /// </summary>
    public static BattleState CheckPhase()
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

    public static void BuildTeamOrder() {

        allySide = new List<PlayerCharacter>() { DBPlayerCharacter.GetPC(13), DBPlayerCharacter.GetPC(10), DBPlayerCharacter.GetPC(11), DBPlayerCharacter.GetPC(40) };
        enemySide = new List<PlayerCharacter>() { DBPlayerCharacter.GetPC(1), DBPlayerCharacter.GetPC(17), DBPlayerCharacter.GetPC(31), DBPlayerCharacter.GetPC(420), DBPlayerCharacter.GetPC(666) }; //Ally side and enemy side should be set outside of this script, this is here for testing reasons


        for (int i = 0; i < allySide.Count; i++)
        {
            teamOrder.Add(allySide[i]);
            allySide[i].team = Character.Team.Ally;
        }

        for (int i = 0; i < enemySide.Count; i++)
        {
            teamOrder.Add(enemySide[i]);
            enemySide[i].team = Character.Team.Enemy;
        }

        phaseChangeIndex = allySide.Count;
    }
}
