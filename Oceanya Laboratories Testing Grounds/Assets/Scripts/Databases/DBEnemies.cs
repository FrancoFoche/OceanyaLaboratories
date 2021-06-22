using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBEnemies : MonoBehaviour
{
    public static List<Enemy> enemies { get; private set; }
    public static void BuildDatabase()
    {
        enemies = new List<Enemy>()
        {
            new Enemy(1 , "Motivated Swordmaster" , GameAssetsManager.instance.GetSprite(Sprites.Vergil),
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , 100   },
                    { Stats.CURHP       , 100   },
                    { Stats.STR         , 15    },
                    { Stats.INT         , 1     },
                    { Stats.CHR         , 1     },
                    { Stats.AGI         , 25    },
                    { Stats.MR          , 0     },
                    { Stats.PR          , 0     },
                    { Stats.CON         , 30    },
                    { Stats.HPREGEN     , 0     }
                },
                new List<Skill>()
                {
                    new Skill(new BaseObjectInfo("Great Sword Slash", 1, "You swing your sword precisely and smoothly, dealing 150% STR to a single target")
                    ,"_caster_ activates Great Sword Slash! They swing their sword precisely and stylishly at _target_! _caster_ receives _damage_ DMG."
                    ,ActivatableType.Active
                    ,TargetType.Single)
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Physical, ElementType.Dark, new List<RPGFormula>(){ new RPGFormula(Stats.STR,operationActions.Multiply,1.5f)})
                    .BehaviorHasCooldown(CDType.Turns, 1),

                    new Skill(new BaseObjectInfo("Now i'm a little MOTIVATED!", 2, "You realize this fight is finally starting to be worth your time. You stab yourself for 50% your current HP in order to gain +100% STR (This skill can only be activated when below 50% HP)")
                    ,"_caster_ looks at their wounds, they're finally starting to feel MOTIVATED! They stab themselves for _damage_ DMG and receive a 100% STR buff from their sword's power!"
                    ,ActivatableType.Active
                    ,TargetType.Self)
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Physical, ElementType.Dark, new List<RPGFormula>(){ new RPGFormula(Stats.CURHP,operationActions.Multiply,0.5f)})
                    .BehaviorHasCooldown(CDType.Other)
                    .BehaviorModifiesStat(StatModificationTypes.Buff, new Dictionary<Stats, List<RPGFormula>>(){ { Stats.STR, new List<RPGFormula>(){new RPGFormula(Stats.STR, operationActions.Multiply, 1f) } } })
                    .BehaviorActivationRequirement(new List<ActivationRequirement>(){new ActivationRequirement(Stats.CURHP, ActivationRequirement.ComparerType.LessThan, new RPGFormula(Stats.MAXHP,operationActions.Multiply, 0.5f))}),

                    new Skill(new BaseObjectInfo("Death Sentence", 3, "You point your sword at two unlucky foes. They will get slashed for 100% your STR")
                    ,"_caster_ points their sword at _target_, oh no. Shortly after, they receive _damage_ DMG from DEATH SENTENCE."
                    ,ActivatableType.Active
                    ,TargetType.Multiple, 2)
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Physical, ElementType.Dark, new List<RPGFormula>(){ new RPGFormula(Stats.STR, operationActions.Multiply,1f)})
                    .BehaviorHasCooldown(CDType.Turns, 3)
                },
                new Dictionary<Item, int>()
                {

                }
            ),

            new Enemy(2 , "Strategist" ,GameAssetsManager.instance.GetSprite(Sprites.Sasque),
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , 150 },
                    { Stats.CURHP       , 150 },
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
                    GameAssetsManager.instance.GetSkill(ClassNames.MasterOfDarkArts.ToString(),"Mind Over Body"),
                    new Skill(new BaseObjectInfo("Physical Attack Formation", 1, "You command your team into a physical attack formation, making them gain a +10 STR Buff")
                    ,"_target_ moves into a Physical Attack Formation, as per _caster_'s request! They gain a buff for STR"
                    ,ActivatableType.Active
                    ,TargetType.AllAllies)
                    .BehaviorCostsTurn()
                    .BehaviorModifiesStat(StatModificationTypes.Buff, new Dictionary<Stats, int>(){ { Stats.STR, 10 } })
                    .BehaviorHasCooldown(CDType.Other),

                    new Skill(new BaseObjectInfo("Magic Attack Formation", 1, "You command your team into a magic attack formation, making them gain an INT Buff equal to 50% your INT")
                    ,"_target_ moves into a Magical Attack Formation, as per _caster_'s request! They gain a buff for INT"
                    ,ActivatableType.Active
                    ,TargetType.AllAllies)
                    .BehaviorCostsTurn()
                    .BehaviorModifiesStat(StatModificationTypes.Buff, new Dictionary<Stats, int>(){ { Stats.INT, 10 } })
                    .BehaviorHasCooldown(CDType.Other),

                    new Skill(new BaseObjectInfo("Support Formation", 1, "You command your team to fall back and wait to get healed, everyone gets healed by 50 HP")
                    ,"_target_ moves into a Support Formation, as per _caster_'s request! They get healed by +25% their MAXHP"
                    ,ActivatableType.Active
                    ,TargetType.AllAllies)
                    .BehaviorCostsTurn()
                    .BehaviorDoesHeal(50)
                    .BehaviorHasCooldown(CDType.Turns,1),

                    new Skill(new BaseObjectInfo("Defense Formation", 1, "You command your team into a defensive formation, making them gain an MR and PR Buff equal to 100")
                    ,"_target_ moves into a Defensive Formation, as per _caster_'s request! They receive an MR and PR buff!"
                    ,ActivatableType.Active
                    ,TargetType.AllAllies)
                    .BehaviorCostsTurn()
                    .BehaviorModifiesStat(StatModificationTypes.Buff, 
                    new Dictionary<Stats, int>(){ 
                        { Stats.MR, 100 },
                        { Stats.PR, 100 } 
                    })
                    .BehaviorHasCooldown(CDType.Other),
                }, 
                new Dictionary<Item, int>()
                {

                }
            ),

            new Enemy(3 , "Kirbo" ,GameAssetsManager.instance.GetSprite(Sprites.Kirbo),
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
                    { Stats.HPREGEN     , 0 }
                },
                new List<Skill>()
                {
                    new Skill(new BaseObjectInfo("Pink counter", 1, "Attacks bounce off of your body, and reflect back at the enemy for 10% your CON stat!")
                    ,"Poyo! (Kirbo exploded, making _target_ receive _damage_ DMG, huh.)"
                    ,ActivatableType.Passive
                    ,TargetType.Bounce)
                    .BehaviorDoesDamage(DamageType.Physical, ElementType.Dark, 30)
                    .BehaviorPassive(ActivationTime.WhenAttacked)
                },
                new Dictionary<Item, int>()
                {

                }
            ),
        };
    }
}