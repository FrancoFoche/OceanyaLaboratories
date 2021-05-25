using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBEnemies : MonoBehaviour
{
    public static List<Enemy> enemies = new List<Enemy>();
    public static void BuildDatabase()
    {
        enemies = new List<Enemy>()
        {
            new Enemy(1 , "TankDummy" , GameAssetsManager.instance.GetSprite(Sprites.MagnoDrip), 10,
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , 200   },
                    { Stats.CURHP       , 200   },
                    { Stats.STR         , 20    },
                    { Stats.INT         , 1     },
                    { Stats.CHR         , 1     },
                    { Stats.AGI         , 25     },
                    { Stats.MR          , 100   },
                    { Stats.PR          , 100   },
                    { Stats.CON         , 30     },
                    { Stats.HPREGEN     , 0     }
                },
                new List<Skill>()
                {
                    DBSkills.GetSkill(SenjutsuSubclasses.FrogStyleSage.ToString(),"Great Fire Ball"),
                }, new List<Item>()
            ),

            new Enemy(2 , "Wizard Dummy" ,GameAssetsManager.instance.GetSprite(Sprites.MagnoDrip), 10,
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , 100 },
                    { Stats.CURHP       , 100 },
                    { Stats.STR         , 1 },
                    { Stats.INT         , 20 },
                    { Stats.CHR         , 1 },
                    { Stats.AGI         , 30 },
                    { Stats.MR          , 0 },
                    { Stats.PR          , 0 },
                    { Stats.CON         , 1 },
                    { Stats.HPREGEN     , 0 }
                },
                new List<Skill>()
                {
                    DBSkills.GetSkill(ClassNames.MasterOfDarkArts.ToString(),"Mind Over Body"),
                    DBSkills.GetSkill(SenjutsuSubclasses.FrogStyleSage.ToString(),"Triple Threat"),
                }, new List<Item>()
            ),

            new Enemy(3 , "Archwizard Dummy" ,GameAssetsManager.instance.GetSprite(Sprites.MagnoDrip), 20,
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , 150 },
                    { Stats.CURHP       , 150 },
                    { Stats.STR         , 1 },
                    { Stats.INT         , 20 },
                    { Stats.CHR         , 1 },
                    { Stats.AGI         , 50 },
                    { Stats.MR          , 100 },
                    { Stats.PR          , 100 },
                    { Stats.CON         , 1 },
                    { Stats.HPREGEN     , 0 }
                },
                new List<Skill>()
                {
                    DBSkills.GetSkill(ClassNames.MasterOfDarkArts.ToString(),"Mind Over Body"),
                    DBSkills.GetSkill(SenjutsuSubclasses.FrogStyleSage.ToString(),"Triple Threat"),
                    DBSkills.GetSkill(ClassNames.MasterOfDarkArts.ToString(),"White Dragon Breath"),
                    DBSkills.GetSkill(ClassNames.MasterOfDarkArts.ToString(),"Soul Spear"),
                }, new List<Item>()
            ),
        };
    }

    public static Enemy GetEnemy(int id)
    {
        return enemies.Find(resultEnemy => resultEnemy.ID == id);
    }

    public static Enemy GetEnemy(string name)
    {
        return enemies.Find(resultEnemy => resultEnemy.name == name);
    }
}