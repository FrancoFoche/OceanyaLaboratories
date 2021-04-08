using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Storage of all player characters
/// </summary>
public class DBPlayerCharacter : MonoBehaviour
{
    public static List<PlayerCharacter> pCharacters = new List<PlayerCharacter>();


    /// <summary>
    /// Builds the initial database to start pulling information from.
    /// </summary>
    public static void BuildDatabase()
    {
        pCharacters = new List<PlayerCharacter>()
        {
            new PlayerCharacter(13 , "Vinnie" , 1,  DBClasses.GetClass(0)/*nobody, change later to vampire*/ ,
                new Dictionary<Character.Stats, int>
                {
                    { Character.Stats.MAXHP       , 47 },
                    { Character.Stats.CURHP       , 47 },
                    { Character.Stats.STR      , 10 },
                    { Character.Stats.INT      , 3  },
                    { Character.Stats.CHR      , 13 },
                    { Character.Stats.AGI      , 35 },
                    { Character.Stats.MR       , 16 },
                    { Character.Stats.PR       , 15 },
                    { Character.Stats.CON      , 9  },
                    { Character.Stats.HPREGEN  , 0  }
                },
                new List<Skill>()
                {
                    DBSkills.GetSkill(1,0),
                    DBSkills.GetSkill(1,1),
                    DBSkills.GetSkill(1,10)
                },
                new List<Skill>()
            ),

            new PlayerCharacter(1 , "Tank" , 1, DBClasses.GetClass(0)/*nobody, change later to vampire*/ ,
                new Dictionary<Character.Stats, int>
                {
                    { Character.Stats.MAXHP       , 200 },
                    { Character.Stats.CURHP       , 200 },
                    { Character.Stats.STR      , 20 },
                    { Character.Stats.INT      , 0  },
                    { Character.Stats.CHR      , 0 },
                    { Character.Stats.AGI      , 0 },
                    { Character.Stats.MR       , 100 },
                    { Character.Stats.PR       , 100 },
                    { Character.Stats.CON      , 0  },
                    { Character.Stats.HPREGEN  , 0  }
                },
                new List<Skill>()
                {
                    DBSkills.GetSkill(1,10)
                },
                new List<Skill>()
            ),

            new PlayerCharacter(2 , "Borracho" , 1, DBClasses.GetClass(0)/*nobody, change later to vampire*/ ,
                new Dictionary<Character.Stats, int>
                {
                    { Character.Stats.MAXHP       , 100 },
                    { Character.Stats.CURHP       , 100 },
                    { Character.Stats.STR      , 5 },
                    { Character.Stats.INT      , 0  },
                    { Character.Stats.CHR      , 0 },
                    { Character.Stats.AGI      , 0 },
                    { Character.Stats.MR       , 200 },
                    { Character.Stats.PR       , 200 },
                    { Character.Stats.CON      , 0  },
                    { Character.Stats.HPREGEN  , 0  }
                },
                new List<Skill>()
                {
                },
                new List<Skill>()
            )
        };
    }

    /// <summary>
    /// Iterates through the database to get a player character with the id you gave it
    /// </summary>
    /// <param name="id">ID of the character you want</param>
    /// <returns>PlayerCharacter</returns>
    public static PlayerCharacter GetPC(int id)
    {
        return pCharacters.Find(playercharacter => playercharacter.ID == id);
    }

    /// <summary>
    /// Iterates through the database to get a player character with the name you gave it
    /// </summary>
    /// <param name="name">Name of the character you want</param>
    /// <returns>PlayerCharacter</returns>
    public static PlayerCharacter GetPC(string name)
    {
        return pCharacters.Find(playercharacter => playercharacter.name == name);
    }
}