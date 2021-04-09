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

            //HEROES
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
            new PlayerCharacter(10 , "Cientifico" , 1,  DBClasses.GetClass(0)/*nobody, change later to vampire*/ ,
                new Dictionary<Character.Stats, int>
                {
                    { Character.Stats.MAXHP       , 1 },
                    { Character.Stats.CURHP       , 1 },
                    { Character.Stats.STR      , 1 },
                    { Character.Stats.INT      , 300  },
                    { Character.Stats.CHR      , 0 },
                    { Character.Stats.AGI      , 1 },
                    { Character.Stats.MR       , 0 },
                    { Character.Stats.PR       , 0 },
                    { Character.Stats.CON      , 0  },
                    { Character.Stats.HPREGEN  , 0  }
                },
                new List<Skill>()
                {
                    
                },
                new List<Skill>()
            ),
            new PlayerCharacter(11 , "Programador" , 1,  DBClasses.GetClass(0)/*nobody, change later to vampire*/ ,
                new Dictionary<Character.Stats, int>
                {
                    { Character.Stats.MAXHP       , 1 },
                    { Character.Stats.CURHP       , 1 },
                    { Character.Stats.STR      , 1 },
                    { Character.Stats.INT      , 300  },
                    { Character.Stats.CHR      , 0 },
                    { Character.Stats.AGI      , 1 },
                    { Character.Stats.MR       , 0 },
                    { Character.Stats.PR       , 0 },
                    { Character.Stats.CON      , 0  },
                    { Character.Stats.HPREGEN  , 0  }
                },
                new List<Skill>()
                {

                },
                new List<Skill>()
            ),

            new PlayerCharacter(40 , "Magno" , 999,  DBClasses.GetClass(0)/*nobody, change later to vampire*/ ,
                new Dictionary<Character.Stats, int>
                {
                    { Character.Stats.MAXHP       , 100000000 },
                    { Character.Stats.CURHP       , 100000000 },
                    { Character.Stats.STR      , 100000000 },
                    { Character.Stats.INT      , 100000000  },
                    { Character.Stats.CHR      , 100000000 },
                    { Character.Stats.AGI      , 100000000 },
                    { Character.Stats.MR       , 100000000 },
                    { Character.Stats.PR       , 100000000 },
                    { Character.Stats.CON      , 100000000  },
                    { Character.Stats.HPREGEN  , 100000000  }
                },
                new List<Skill>()
                {

                },
                new List<Skill>()
            ),

            //ENEMIGOS
            new PlayerCharacter(1 , "Tank" , 10, DBClasses.GetClass(0)/*nobody, change later to vampire*/ ,
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

            new PlayerCharacter(666 , "Castro (el de las coins)" , 999, DBClasses.GetClass(0)/*nobody, change later to vampire*/ ,
                new Dictionary<Character.Stats, int>
                {
                    { Character.Stats.MAXHP       , 1000 },
                    { Character.Stats.CURHP       , 1000 },
                    { Character.Stats.STR      , 100 },
                    { Character.Stats.INT      , 200  },
                    { Character.Stats.CHR      , 99999 },
                    { Character.Stats.AGI      , 80 },
                    { Character.Stats.MR       , 100 },
                    { Character.Stats.PR       , 100 },
                    { Character.Stats.CON      , 2000  },
                    { Character.Stats.HPREGEN  , 1000  }
                },
                new List<Skill>()
                {
                },
                new List<Skill>()
            ),

            new PlayerCharacter(420 , "DaBaby (ConvertibleForm)" , 10, DBClasses.GetClass(0)/*nobody, change later to vampire*/ ,
                new Dictionary<Character.Stats, int>
                {
                    { Character.Stats.MAXHP       , 200 },
                    { Character.Stats.CURHP       , 200 },
                    { Character.Stats.STR      , 1 },
                    { Character.Stats.INT      , 1  },
                    { Character.Stats.CHR      , 1 },
                    { Character.Stats.AGI      , 10000 },
                    { Character.Stats.MR       , 1 },
                    { Character.Stats.PR       , 1 },
                    { Character.Stats.CON      , 20  },
                    { Character.Stats.HPREGEN  , 0  }
                },
                new List<Skill>()
                {
                },
                new List<Skill>()
            ),

            new PlayerCharacter(17 , "Sans SmashBros" , 1, DBClasses.GetClass(0)/*nobody, change later to vampire*/ ,
                new Dictionary<Character.Stats, int>
                {
                    { Character.Stats.MAXHP       , 1 },
                    { Character.Stats.CURHP       , 1 },
                    { Character.Stats.STR      , 1 },
                    { Character.Stats.INT      , 1  },
                    { Character.Stats.CHR      , 1 },
                    { Character.Stats.AGI      , 1 },
                    { Character.Stats.MR       , 1 },
                    { Character.Stats.PR       , 1 },
                    { Character.Stats.CON      , 1  },
                    { Character.Stats.HPREGEN  , 1  }
                },
                new List<Skill>()
                {
                },
                new List<Skill>()
            ),

            new PlayerCharacter(31 , "Edmund McNerfs" , 8, DBClasses.GetClass(0)/*nobody, change later to vampire*/ ,
                new Dictionary<Character.Stats, int>
                {
                    { Character.Stats.MAXHP       , Random.Range(1,1001) },
                    { Character.Stats.CURHP       , Random.Range(1,1001) },
                    { Character.Stats.STR      , Random.Range(1,1001) },
                    { Character.Stats.INT      , Random.Range(1,1001)  },
                    { Character.Stats.CHR      , Random.Range(1,1001) },
                    { Character.Stats.AGI      , Random.Range(1,1001) },
                    { Character.Stats.MR       , Random.Range(1,201) },
                    { Character.Stats.PR       , Random.Range(1,201) },
                    { Character.Stats.CON      , Random.Range(1,1001)  },
                    { Character.Stats.HPREGEN  , Random.Range(1,1001)  }
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