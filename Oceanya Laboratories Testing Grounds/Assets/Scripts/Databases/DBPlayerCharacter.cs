using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

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

            new PlayerCharacter(0 , "TestDummy" , 1, GameAssetsManager.instance.GetClass(0),
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , 100 },
                    { Stats.CURHP       , 100 },
                    { Stats.STR      , 100 },
                    { Stats.INT      , 100  },
                    { Stats.CHR      , 100 },
                    { Stats.AGI      , 100 },
                    { Stats.MR       , 100 },
                    { Stats.PR       , 100 },
                    { Stats.CON      , 100  },
                    { Stats.HPREGEN  , 100  }
                },
                GameAssetsManager.instance.GetClass(0).skillList,
                new List<Item>
                {
                }
            ),

            new PlayerCharacter(13 , "Vinnie" , 1,  GameAssetsManager.instance.GetClass(ClassNames.Vampire.ToString()) ,
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , 47 },
                    { Stats.CURHP       , 47 },
                    { Stats.STR      , 10 },
                    { Stats.INT      , 20  },
                    { Stats.CHR      , 30 },
                    { Stats.AGI      , 35 },
                    { Stats.MR       , 16 },
                    { Stats.PR       , 15 },
                    { Stats.CON      , 9  },
                    { Stats.HPREGEN  , 0  }
                },

                new List<Skill>()
                {
                    GameAssetsManager.instance.GetSkill(ClassNames.Vampire.ToString(),"Vampire Fangs"),
                    GameAssetsManager.instance.GetSkill(ClassNames.Vampire.ToString(),"Bat Swarm"),
                    GameAssetsManager.instance.GetSkill(ClassNames.Vampire.ToString(),"Dry their blood"),
                    GameAssetsManager.instance.GetSkill(ClassNames.Doctor.ToString(),"Heal"),
                },
                new List<Item>
                {

                }
            ),
            new PlayerCharacter(5 , "Da Docta" , 9,  GameAssetsManager.instance.GetClass(ClassNames.Doctor.ToString()),
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , 83    },
                    { Stats.CURHP       , 83    },
                    { Stats.STR         , 1     },
                    { Stats.INT         , 57    },
                    { Stats.CHR         , 9     },
                    { Stats.AGI         , 37    },
                    { Stats.MR          , 2     },
                    { Stats.PR          , 3     },
                    { Stats.CON         , 13    },
                    { Stats.HPREGEN     , 0     }
                },
                new List<Skill>()
                {
                    GameAssetsManager.instance.GetSkill(ClassNames.Doctor.ToString(),"Chakra Scalples"),
                    GameAssetsManager.instance.GetSkill(ClassNames.Doctor.ToString(),"Revival Ritual"),
                    GameAssetsManager.instance.GetSkill(ClassNames.Doctor.ToString(),"Heal"),
                    GameAssetsManager.instance.GetSkill("Testing Class","Regenerative Meditation"),
                },
                new List<Item>()
            ),

            new PlayerCharacter(9 , "Archive" , 5,  GameAssetsManager.instance.GetClass(ClassNames.MasterOfDarkArts.ToString()) ,
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , 50    },
                    { Stats.CURHP       , 50    },
                    { Stats.STR         , 1     },
                    { Stats.INT         , 36    },
                    { Stats.CHR         , 2     },
                    { Stats.AGI         , 30    },
                    { Stats.MR          , 10    },
                    { Stats.PR          , 5     },
                    { Stats.CON         , 6     },
                    { Stats.HPREGEN     , 0     }
                },
                new List<Skill>()
                {

                    GameAssetsManager.instance.GetSkill(ClassNames.MasterOfDarkArts.ToString(),"Mind Over Body"),
                    GameAssetsManager.instance.GetSkill(SenjutsuSubclasses.FrogStyleSage.ToString(),"Triple Threat"),
                    GameAssetsManager.instance.GetSkill(ClassNames.MasterOfDarkArts.ToString(),"White Dragon Breath"),
                    GameAssetsManager.instance.GetSkill(ClassNames.MasterOfDarkArts.ToString(),"Soul Spear"),
                    GameAssetsManager.instance.GetSkill("Testing Class","Arcane Overflow"),
                },
                new List<Item>()
            ),
        };
    }
}