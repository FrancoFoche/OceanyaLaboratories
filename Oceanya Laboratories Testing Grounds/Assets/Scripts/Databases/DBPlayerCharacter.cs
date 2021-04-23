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
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , 47 },
                    { Stats.CURHP       , 47 },
                    { Stats.STR      , 10 },
                    { Stats.INT      , 3  },
                    { Stats.CHR      , 13 },
                    { Stats.AGI      , 35 },
                    { Stats.MR       , 16 },
                    { Stats.PR       , 15 },
                    { Stats.CON      , 9  },
                    { Stats.HPREGEN  , 0  }
                },
                DBSkills.GetAllSkills(),
                new List<Skill>()
            ),
            new PlayerCharacter(10 , "Cientifico" , 1,  DBClasses.GetClass(0)/*nobody, change later to vampire*/ ,
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , 1 },
                    { Stats.CURHP       , 1 },
                    { Stats.STR      , 1 },
                    { Stats.INT      , 300  },
                    { Stats.CHR      , 0 },
                    { Stats.AGI      , 1 },
                    { Stats.MR       , 0 },
                    { Stats.PR       , 0 },
                    { Stats.CON      , 0  },
                    { Stats.HPREGEN  , 0  }
                },
                new List<Skill>()
                {
                    
                },
                new List<Skill>()
            ),
            new PlayerCharacter(11 , "Programador" , 1,  DBClasses.GetClass(0)/*nobody, change later to vampire*/ ,
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , 1 },
                    { Stats.CURHP       , 1 },
                    { Stats.STR      , 1 },
                    { Stats.INT      , 300  },
                    { Stats.CHR      , 0 },
                    { Stats.AGI      , 1 },
                    { Stats.MR       , 0 },
                    { Stats.PR       , 0 },
                    { Stats.CON      , 0  },
                    { Stats.HPREGEN  , 0  }
                },
                new List<Skill>()
                {

                },
                new List<Skill>()
            ),

            new PlayerCharacter(40 , "Magno" , 999,  DBClasses.GetClass(0)/*nobody, change later to vampire*/ ,
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , 100000000 },
                    { Stats.CURHP       , 100000000 },
                    { Stats.STR      , 100000000 },
                    { Stats.INT      , 100000000  },
                    { Stats.CHR      , 100000000 },
                    { Stats.AGI      , 100000000 },
                    { Stats.MR       , 100000000 },
                    { Stats.PR       , 100000000 },
                    { Stats.CON      , 100000000  },
                    { Stats.HPREGEN  , 100000000  }
                },
                new List<Skill>()
                {

                },
                new List<Skill>()
            ),

            //ENEMIGOS
            new PlayerCharacter(1 , "Tank" , 10, DBClasses.GetClass(0)/*nobody, change later to vampire*/ ,
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , 200 },
                    { Stats.CURHP       , 200 },
                    { Stats.STR      , 20 },
                    { Stats.INT      , 0  },
                    { Stats.CHR      , 0 },
                    { Stats.AGI      , 0 },
                    { Stats.MR       , 100 },
                    { Stats.PR       , 100 },
                    { Stats.CON      , 0  },
                    { Stats.HPREGEN  , 0  }
                },
                new List<Skill>()
                {
                    DBSkills.GetSkill(1,10)
                },
                new List<Skill>()
            ),

            new PlayerCharacter(666 , "Castro (el de las coins)" , 999, DBClasses.GetClass(0)/*nobody, change later to vampire*/ ,
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , 1000 },
                    { Stats.CURHP       , 1000 },
                    { Stats.STR      , 100 },
                    { Stats.INT      , 200  },
                    { Stats.CHR      , 99999 },
                    { Stats.AGI      , 80 },
                    { Stats.MR       , 100 },
                    { Stats.PR       , 100 },
                    { Stats.CON      , 2000  },
                    { Stats.HPREGEN  , 1000  }
                },
                new List<Skill>()
                {
                },
                new List<Skill>()
            ),

            new PlayerCharacter(420 , "DaBaby (ConvertibleForm)" , 10, DBClasses.GetClass(0)/*nobody, change later to vampire*/ ,
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , 200 },
                    { Stats.CURHP       , 200 },
                    { Stats.STR      , 1 },
                    { Stats.INT      , 1  },
                    { Stats.CHR      , 1 },
                    { Stats.AGI      , 10000 },
                    { Stats.MR       , 1 },
                    { Stats.PR       , 1 },
                    { Stats.CON      , 20  },
                    { Stats.HPREGEN  , 0  }
                },
                new List<Skill>()
                {
                },
                new List<Skill>()
            ),

            new PlayerCharacter(17 , "Sans SmashBros" , 1, DBClasses.GetClass(0)/*nobody, change later to vampire*/ ,
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , 1 },
                    { Stats.CURHP       , 1 },
                    { Stats.STR         , 1 },
                    { Stats.INT         , 1 },
                    { Stats.CHR         , 1 },
                    { Stats.AGI         , 1 },
                    { Stats.MR          , 1 },
                    { Stats.PR          , 1 },
                    { Stats.CON         , 1 },
                    { Stats.HPREGEN     , 1 }
                },
                new List<Skill>()
                {
                },
                new List<Skill>()
            ),

            new PlayerCharacter(31 , "Edmund McNerfs" , 8, DBClasses.GetClass(0)/*nobody, change later to vampire*/ ,
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , Random.Range(1,1001) },
                    { Stats.CURHP       , Random.Range(1,1001) },
                    { Stats.STR      , Random.Range(1,1001) },
                    { Stats.INT      , Random.Range(1,1001)  },
                    { Stats.CHR      , Random.Range(1,1001) },
                    { Stats.AGI      , Random.Range(1,1001) },
                    { Stats.MR       , Random.Range(1,201) },
                    { Stats.PR       , Random.Range(1,201) },
                    { Stats.CON      , Random.Range(1,1001)  },
                    { Stats.HPREGEN  , Random.Range(1,1001)  }
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