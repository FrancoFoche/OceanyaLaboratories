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
            new Enemy(1 , "Tank" , GameAssetsManager.instance.GetSprite(Sprites.MagnoDrip), 10,
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

                }
            ),

            new Enemy(666 , "Castro (el de las coins)" ,GameAssetsManager.instance.GetSprite(Sprites.MagnoDrip), 999,
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

                }
            ),

            new Enemy(420 , "DaBaby (ConvertibleForm)" ,GameAssetsManager.instance.GetSprite(Sprites.MagnoDrip), 10,
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

                }
            ),

            new Enemy(17 , "Sans SmashBros" ,GameAssetsManager.instance.GetSprite(Sprites.MagnoDrip), 1,
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

                }
            ),

            new Enemy(31 , "Edmund McNerfs" ,GameAssetsManager.instance.GetSprite(Sprites.MagnoDrip), 8,
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

                }
            )
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