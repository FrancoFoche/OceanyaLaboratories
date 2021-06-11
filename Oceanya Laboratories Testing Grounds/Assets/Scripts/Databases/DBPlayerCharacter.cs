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
                new Dictionary<Item, int>()
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
                    new Skill
                    (
                            new BaseObjectInfo("Vampire Fangs", 1 , "You utilize your vampire fangs to attack! Your basic attacks deal 50% STR + 50% CHR, Vampires have this skill by default")
                            ,"_caster_ remembers that they're a vampire! Bite the enemy! Their basic attacks will now deal 50% STR + 50% CHR"
                            ,ActivatableType.Passive
                            ,TargetType.Self
                    )
                    .BehaviorPassive(ActivationTime.StartOfBattle)
                    .BehaviorChangesBasicAttack(new List<RPGFormula>(){new RPGFormula(Stats.STR,operationActions.Multiply,0.5f), new RPGFormula(Stats.CHR,operationActions.Multiply,0.5f)}, DamageType.Physical)
                    ,
                    new Skill
                    (
                            new BaseObjectInfo("Bat Swarm", 2 , "Call your Bat friends to attack the enemy team for 25% your CHR! 2 Turn CD")
                            ,"_caster_'s bat friends come help! _target_ receives _damage_ DMG!"
                            ,ActivatableType.Active
                            ,TargetType.AllEnemies
                    )
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Physical,ElementType.Normal,new List<RPGFormula>(){new RPGFormula(Stats.CHR,operationActions.Multiply,0.25f) })
                    .BehaviorHasCooldown(CDType.Turns,2)
                    ,
                    new Skill
                    (
                            new BaseObjectInfo("Dry their blood", 3 , "You suck the blood out of the enemy, leaving them weaker. Their STR is debuffed by 50% your CHR. You can only use this skill once.")
                            ,"_caster_ weakens _target_! Their STR is debuffed by 50% of _caster_'s CHR!"
                            ,ActivatableType.Active
                            ,TargetType.Single
                    )
                    .BehaviorCostsTurn()
                    .BehaviorModifiesStat(StatModificationTypes.Debuff,new Dictionary<Stats, List<RPGFormula>>(){ { Stats.STR, new List<RPGFormula>{new RPGFormula(Stats.CHR, operationActions.Multiply, 0.5f)} } })
                    .BehaviorHasCooldown(CDType.Other)
                },
                new Dictionary<Item, int>()
                {
                    { GameAssetsManager.instance.GetItem("Intelligence Potion") , 2},
                    { GameAssetsManager.instance.GetItem("Fresh Blood") , 2}
                }
            ),
            new PlayerCharacter(5 , "Da Docta" , 1,  GameAssetsManager.instance.GetClass(ClassNames.Doctor.ToString()),
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , 83    },
                    { Stats.CURHP       , 83    },
                    { Stats.STR         , 1     },
                    { Stats.INT         , 50    },
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
                new Dictionary<Item, int>()
                {
                    { GameAssetsManager.instance.GetItem("HP Pot+") , 3},
                    { GameAssetsManager.instance.GetItem("HP Pot") , 1}
                }
            ),

            new PlayerCharacter(9 , "Archive" , 1,  GameAssetsManager.instance.GetClass(ClassNames.MasterOfDarkArts.ToString()) ,
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , 50    },
                    { Stats.CURHP       , 50    },
                    { Stats.STR         , 1     },
                    { Stats.INT         , 40    },
                    { Stats.CHR         , 0     },
                    { Stats.AGI         , 30    },
                    { Stats.MR          , 10    },
                    { Stats.PR          , 5     },
                    { Stats.CON         , 6     },
                    { Stats.HPREGEN     , 0     }
                },
                new List<Skill>()
                {
                    new Skill
                    (
                        new BaseObjectInfo("Mind Over Body", 1 , "Your attacks are INT based instead of STR based, Masters of Dark Arts have this by default.")
                        ,"_caster_'s mind is more powerful than their body. Their base attacks now deal 100% INT as magic damage!"
                        ,ActivatableType.Passive
                        ,TargetType.Self
                    )
                    .BehaviorPassive(ActivationTime.StartOfBattle)
                    .BehaviorChangesBasicAttack(new List<RPGFormula>(){new RPGFormula(Stats.INT,operationActions.Multiply,1f)}, DamageType.Magical)
                    ,
                    new Skill //needs an activation requirement, needs an applies status effect, needs a lasts for
                    (
                        new BaseObjectInfo("Triple Threat", 2 , "You cast three elements and deal 200% your INT as Magic Damage! (2 Turn CD)")
                        ,"_caster_ casts three elements, all meant to fuck _target_ up! _damage_ DMG"
                        ,ActivatableType.Active
                        ,TargetType.Single
                    )
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Magical,ElementType.Fire, new List<RPGFormula>(){ new RPGFormula(Stats.INT, operationActions.Multiply, 2)})
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -3 }
                        }
                    )
                    .BehaviorHasCooldown(CDType.Turns,2)
                    ,
                    new Skill
                    (
                        new BaseObjectInfo("White Dragon Breath", 3 , "You channel the energy of the great white dragon (not necessarily one with blue eyes) to unleash a powerful ice barrage! All enemies get hit with 75% your INT.")
                        ,"_caster_ channels the energy of the great white dragon! _target_ gets hit by _damage_!"
                        ,ActivatableType.Active
                        ,TargetType.AllEnemies
                    )
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Magical,ElementType.Ice, new List<RPGFormula>(){ new RPGFormula(Stats.INT,operationActions.Multiply,0.75f)})
                    .BehaviorHasCooldown(CDType.Turns,3)
                    ,
                    new Skill
                    (
                        new BaseObjectInfo("Soul Spear", 4 , "You materialize your soul's will into a powerful Spear that strikes through your enemy's soul! It deals 150% INT")
                        ,"_caster_ manifests a Soul Spear! _target_ receives _damage_ DMG!"
                        ,ActivatableType.Active
                        ,TargetType.Single
                    )
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Magical,ElementType.Normal, new List<RPGFormula>(){ new RPGFormula(Stats.INT,operationActions.Multiply,1.5f)})
                    .BehaviorHasCooldown(CDType.Turns,2)
                    ,
                    new Skill
                    (
                            new BaseObjectInfo("Arcane Overflow", 12 , "You purposefully take your body to its magic limits! You will receive a 50% INT increase at the start of your next 3 turns, BUT you will also receive 20 DIRECT DMG each time.")
                            ,"_caster_'s body is overflowing with energy! Their INT is buffed by 50%, yet their body takes _damage_ DMG."
                            ,ActivatableType.Active
                            ,TargetType.Self
                    )
                    .BehaviorCostsTurn()
                    .BehaviorPassive(ActivationTime.StartOfTurn)
                    .BehaviorModifiesStat(StatModificationTypes.Buff,new Dictionary<Stats, List<RPGFormula>>(){{ Stats.INT, new List<RPGFormula>() { new RPGFormula(Stats.INT, operationActions.Multiply,0.5f)} } })
                    .BehaviorDoesDamage(DamageType.Direct,ElementType.Normal, 20)
                    .BehaviorLastsFor(3)
                    .BehaviorHasCooldown(CDType.Other)
                },
                new Dictionary<Item, int>()
                {
                    { GameAssetsManager.instance.GetItem("HP Pot") , 2},
                    { GameAssetsManager.instance.GetItem("Strength Potion") , 2}
                }
            ),
        };
    }
}