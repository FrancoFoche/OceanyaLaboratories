using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Where all classes, subclasses and their skills are created. Class ID's are defined in order, while Subclass ID's are defined as (ClassID)0(SubclassID)
/// </summary>
public class DBClasses : MonoBehaviour
{
    public static List<BaseSkillClass> classes = new List<BaseSkillClass>();
    public static void BuildDatabase()
    {
        classes = new List<BaseSkillClass>()
        {
            #region Skill Template
            /*

            new Skill
            (
                    new BaseObjectInfo("name", id , "description"),
                    Skill.SkillType, 
                    DBClasses.GetClass(id) 
            )
            .BehaviorCostsTurn() //it ends your turn when you use it
            .BehaviorDoesDamage(Skill.DamageType, Skill.ElementType, 
                new List<SkillFormula>()
                {
                    new SkillFormula(Character.Stats , SkillFormula.operationActions , number) 
                })

            */
            #endregion

            new BaseSkillClass(new BaseObjectInfo("Testing Class", 0 , "This is the class that has every test skill"),
                new List<Skill>
                {
                    new Skill
                    (
                            new BaseObjectInfo("Test Cooldown", 0 , "This skill should be activated, then put on cooldown (making it non interactable) and be usable once 2 turns pass.")
                            ,SkillType.Active
                            ,TargetType.Self
                    )
                    .BehaviorCostsTurn()
                    .BehaviorHasCooldown(CDType.Turns, 2),

                    new Skill
                    (
                            new BaseObjectInfo("Test Single Target Damage", 1 , "This skill should do 50% your STR as direct damage to a single target.")
                            ,SkillType.Active
                            ,TargetType.Single
                    )
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Direct,ElementType.Normal, new List<SkillFormula>(){ new SkillFormula(Stats.STR,operationActions.Multiply,0.5f)}),

                    new Skill
                    (
                            new BaseObjectInfo("Test Multiple Target Damage", 1 , "This skill should do 50% your INT as direct damage to 3 targets of your choosing. (You can choose less by pressing enter when you are done)")
                            ,SkillType.Active
                            ,TargetType.Multiple, 3
                    )
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Direct,ElementType.Normal, new List<SkillFormula>(){ new SkillFormula(Stats.INT,operationActions.Multiply,0.5f)}),

                    new Skill
                    (
                            new BaseObjectInfo("Test AoE Damage", 2 , "This skill should do 50% your CHR as direct damage to every target in the opposing team.")
                            ,SkillType.Active
                            ,TargetType.AllEnemies
                    )
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Direct,ElementType.Normal, new List<SkillFormula>(){ new SkillFormula(Stats.CHR,operationActions.Multiply,0.5f)}),

                    new Skill
                    (
                            new BaseObjectInfo("Test Ally AoE Damage", 3 , "This skill should do 50% your AGI as direct damage to every target in the same team as you.")
                            ,SkillType.Active
                            ,TargetType.AllAllies
                    )
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Direct,ElementType.Normal, new List<SkillFormula>(){ new SkillFormula(Stats.AGI,operationActions.Multiply,0.5f)}),
                }
            ),

            new BaseSkillClass(new BaseObjectInfo(ClassNames.SenjutsuMastery.ToString(), 1, ""),
                new List<Skill>
                {
                    new Skill //Done
                    (
                            new BaseObjectInfo("Unlock Nature Energy", 0 , "Unlocks NATURE ENERGY resource.")
                            ,SkillType.Passive
                            ,TargetType.Self
                    )
                    .BehaviorUnlocksResource(new List<SkillResources>{SkillResources.NatureEnergy})
                    .BehaviorPassive(PassiveActivation.StartOfBattle, TargetType.Self)
                    .IsDone(),

                    //

                    new Skill //Done
                    (
                            new BaseObjectInfo("Gather Nature Energy", 1 , "Your body has gotten used to gathering Nature Energy, you can now stay focused and gather Nature Energy! Even in combat! (Each use of Gather Nature Energy charges 1 Nature Energy Point) (Costs Turn)")
                            ,SkillType.Active
                            ,TargetType.Self
                    )
                    .BehaviorCostsTurn()
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , 1 }
                        }
                    )
                    .IsDone(),
                    
                    //

                    new Skill //Needs a "Lasts For" and an activation requirement
                    (
                            new BaseObjectInfo("Weak Sage Mode", 2 , "Your body has still not found balance between the chakras and your body has partially turned into a frog. Still better than before though? +10%STR +10%HP +5%HP Regeneration (Nature Energy cost: 1)")
                            ,SkillType.Active
                            ,TargetType.Self
                    )
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -1 }
                        }
                    )
                    .BehaviorModifiesStat(
                        new Dictionary<Stats, SkillFormula>()
                        {
                            {Stats.STR , new SkillFormula(Stats.STR, operationActions.Multiply, 0.1f) },
                            {Stats.MAXHP , new SkillFormula(Stats.MAXHP, operationActions.Multiply, 0.1f) },
                            {Stats.HPREGEN , new SkillFormula(Stats.HPREGEN, operationActions.Multiply, 0.05f) }
                        }
                    ),
                    
                    //

                    new Skill //Needs a "Lasts For" and an activation requirement
                    (
                            new BaseObjectInfo("Imperfect Sage Mode", 4 , "Your body has found a better balance between the chakras and you only have SOME frog features. Definitely better than before. +15%STR +15%HP +10%HP Regeneration (Nature Energy cost: 1)")
                            ,SkillType.Active
                            ,TargetType.Self
                    )
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -1 }
                        }
                    )
                    .BehaviorModifiesStat(
                        new Dictionary<Stats, SkillFormula>()
                        {
                            {Stats.STR , new SkillFormula(Stats.STR, operationActions.Multiply, 0.15f) },
                            {Stats.MAXHP , new SkillFormula(Stats.MAXHP, operationActions.Multiply, 0.15f) },
                            {Stats.HPREGEN , new SkillFormula(Stats.HPREGEN, operationActions.Multiply, 0.1f) }
                        }
                    ),
                    
                    //

                    new Skill //Needs a lasts for, an activation requirement, a status effect/subclass change behavior and an add Skill behavior
                    (
                            new BaseObjectInfo("Wood Style! Sage Mode!", 5 , "Your body knows balance! You're as beautiful as ever! You now have access to Wood Sage Arts! Your skills will be heavily based on defense! I guess you got a skill TREE. Get it? Cuz wood. and trees? whatever just have your stats .+20%STR +50%HP +25%HP Regeneration (Lasts 6 Turns) (Nature Energy cost: 2)")
                            ,SkillType.Active
                            ,TargetType.Self
                    )
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -2 }
                        }
                    )
                    .BehaviorModifiesStat(
                        new Dictionary<Stats, SkillFormula>()
                        {
                            {Stats.STR , new SkillFormula(Stats.STR, operationActions.Multiply, 0.2f) },
                            {Stats.MAXHP , new SkillFormula(Stats.MAXHP, operationActions.Multiply, 0.5f) },
                            {Stats.HPREGEN , new SkillFormula(Stats.HPREGEN, operationActions.Multiply, 0.25f) }
                        }
                    ),

                    //

                    new Skill //Needs a lasts for, an activation requirement, a status effect/subclass change behavior and an add Skill behavior
                    (
                            new BaseObjectInfo("Frog Style! Sage Mode!", 6 , "Your body knows balance! You're as beautiful as ever! You get Frog Style Sage Arts! Your skills will be heavily based on Support! +20%STR +20%HP +20%HP Regeneration (Lasts 8 Turns) (Nature Energy cost: 2)")
                            ,SkillType.Active
                            ,TargetType.Self
                    )
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -2 }
                        }
                    )
                    .BehaviorModifiesStat(
                        new Dictionary<Stats, SkillFormula>()
                        {
                            {Stats.STR , new SkillFormula(Stats.STR, operationActions.Multiply, 0.2f) },
                            {Stats.MAXHP , new SkillFormula(Stats.MAXHP, operationActions.Multiply, 0.2f) },
                            {Stats.HPREGEN , new SkillFormula(Stats.HPREGEN, operationActions.Multiply, 0.2f) }
                        }
                    ),
                }
                    
            ),

            
            new BaseSkillClass(new BaseObjectInfo(SenjutsuSubclasses.WoodStyleSage.ToString(), 101 , "Your body knows balance! You're as beautiful as ever! You now have access to Wood Sage Arts! Your skills will be heavily based on defense! I guess you got a skill TREE. Get it? Cuz wood. and trees? whatever just have your stats .+20%STR +50%HP +25%HP Regeneration"),
                new List<Skill>
                {
                    new Skill //Needs a lasts for and an activation requirement
                    (
                            new BaseObjectInfo("Hair Protection", 1 , "Use your Sage Chakra to turn your hair into a protective layer of spikes! Anyone who attacks you will receive 25% of your max HP as Physical Damage (Effect is also applied to Wood Clones) (Sage Mode Turn cost: 2)")
                            ,SkillType.Active
                            ,TargetType.Bounce
                    )
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -2 }
                        }
                    )
                    .BehaviorDoesDamage(
                        DamageType.Physical, ElementType.Normal, new List<SkillFormula>(){ new SkillFormula(Stats.MAXHP, operationActions.Multiply, 0.25f) }
                    )
                    .BehaviorPassive(PassiveActivation.WhenAttacked, TargetType.Bounce)
                    .BehaviorCostsTurn(),

                    //

                    new Skill //needs a summon behavior, and an activation requirement
                    (
                        new BaseObjectInfo("Thousand Hand Buddha", 2 , "Summon the great wood statue of the thousand hand buddha! He has 150% your HP, your resistances (magic resistance, physical resistance) and can tank hits for you OR one of your teammates! (Sage Mode Turn cost: 3)")
                        ,SkillType.Active
                        ,TargetType.Self
                    )
                    .BehaviorCostsTurn()
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -3 }
                        }
                    )
                    ,
                    
                    //

                    new Skill //needs a shield behavior, a way to make the thousand hand buddha disappear upon deactivation and an activation requirement
                    (
                        new BaseObjectInfo("Thousand Hand Protection", 3 , "Utilize all of the hands of the Thousand Hand Buddha and protect all of your team for a single turn! Buddha disappears after this ability. (Can only be used when Sage Art! Thousand Hand Buddha! is active) (Sage Mode Turn cost: 0)")
                        ,SkillType.Active
                        ,TargetType.AllAllies
                    )
                    .BehaviorCostsTurn()
                    ,

                    //

                    new Skill //needs an activation requirement
                    (
                        new BaseObjectInfo("Thousand Hand Barrage", 4 , "Utilize all of the hands of the Thousand Hand Buddha to deal 50% of the Buddha’s HP to a target as physical damage! (Can only be used when Sage Art! Thousand Hand Buddha! is active) (Sage Mode Turn cost: 1)")
                        ,SkillType.Active
                        ,TargetType.Single
                    )
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Physical, ElementType.Normal, new List<SkillFormula>(){ new SkillFormula(Stats.MAXHP, operationActions.Multiply, 0.5f) } )
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -1 }
                        }
                    )
                    ,

                    //

                    new Skill //needs an activation requirement, and a lasts for behavior
                    (
                        new BaseObjectInfo("Hashirama Regeneration", 5 , "Utilize your (maybe illegal) Hashirama Cells to boost your own HP regeneration by 25% for this turn (Sage Mode Turn cost: 1)")
                        ,SkillType.Active
                        ,TargetType.Single
                    )
                    .BehaviorCostsTurn()
                    .BehaviorModifiesStat(new Dictionary<Stats, SkillFormula>(){ { Stats.HPREGEN, new SkillFormula(Stats.HPREGEN, operationActions.Multiply, 0.25f) } })
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -1 }
                        }
                    )
                    ,

                    //

                    new Skill //needs an activation requirement, a way to turn damage the clones take into healing and a way to assign a clone to each member
                    (
                        new BaseObjectInfo("Wood Clones", 6 , "Create 2 Wood Clones with 25% your HP and assign a single one to party members, they will take the next blow for them and heal you for the damage they take (Sage Mode Turn cost: 2)")
                        ,SkillType.Active
                        ,TargetType.Multiple, 2
                    )
                    .BehaviorCostsTurn()
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -2 }
                        }
                    )
                    .BehaviorPassive(PassiveActivation.WhenAttacked,TargetType.Bounce)
                }
            ),

            new BaseSkillClass(new BaseObjectInfo(SenjutsuSubclasses.FrogStyleSage.ToString(), 102 , "Your body knows balance! You're as beautiful as ever! You now have access to Wood Sage Arts! Your skills will be heavily based on defense! I guess you got a skill TREE. Get it? Cuz wood. and trees? whatever just have your stats .+20%STR +50%HP +25%HP Regeneration"),
                new List<Skill>
                {
                    new Skill //needs an activation requirement, and a "change base attack formula" behavior, and also make a way for it to activate only when you turn into a frog sage
                    (
                        new BaseObjectInfo("Frog Kumite", 1 , "Thanks to your Frog Training, you now know the fighting art of Frog Kumite! Your base attacks can't miss and they go through enemy defenses! (Sage Mode Turn cost: 3)")
                        ,SkillType.Active
                        ,TargetType.Single
                    )
                    .BehaviorCostsTurn()
                    .BehaviorPassive(PassiveActivation.Once, TargetType.Single)
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -3 }
                        }
                    ),

                    //

                    new Skill //needs an activation requirement, needs an applies status effect, needs a lasts for
                    (
                        new BaseObjectInfo("Triple Threat", 2 , "You utilize your Sage Mode Chakra to cast three elements (Oil, Wind, Fire) You deal 200% your INT as Fire damage and Fire attacks now deal x2 Damage to the target creature! (Lasts as much as sage mode) (Sage Mode Turn cost: 3)")
                        ,SkillType.Active
                        ,TargetType.Single
                    )
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Magical,ElementType.Fire, new List<SkillFormula>(){ new SkillFormula(Stats.INT, operationActions.Multiply, 2)})
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -3 }
                        }
                    ),

                    //

                    new Skill //needs an activation requirement, needs an applies status effect, needs a lasts for
                    (
                        new BaseObjectInfo("Frog Psalm", 3 , "You utlize the Frog Sages to sing a powerful psalm! Apply CONFUSION to the enemy! (Sage Mode Turn cost: 5)")
                        ,SkillType.Active
                        ,TargetType.Single
                    )
                    .BehaviorCostsTurn()
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -5 }
                        }
                    ),

                    //

                    new Skill //needs an activation requirement,  needs a lasts for
                    (
                        new BaseObjectInfo("Frog Dust Barrier", 4 , "Utilize one of the Frog Sages to create a dust barrier around all of you! -20% AGI to all enemies (Sage Mode Turn cost: 3)")
                        ,SkillType.Active
                        ,TargetType.AllEnemies
                    )
                    .BehaviorCostsTurn()
                    .BehaviorModifiesStat(new Dictionary<Stats, SkillFormula>(){ { Stats.AGI, new SkillFormula(Stats.AGI,operationActions.Multiply,0.2f)} })
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -3 }
                        }
                    ),

                    //

                    new Skill //needs an activation requirement
                    (
                        new BaseObjectInfo("Great Fire Ball", 10 , "Kinda self explanatory isn't it? Does 200% STR btw (Sage Mode Turn cost: 3)")
                        ,SkillType.Active
                        ,TargetType.Single
                    )
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Magical, ElementType.Fire,
                         new List<SkillFormula>()
                         {
                            new SkillFormula(Stats.STR , operationActions.Multiply , 2)
                         })
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -3 }
                        }
                    ),
                }
            ),
        };
    }

    public static BaseSkillClass GetClass(int id)
    {
        return classes.Find(resultClass => resultClass.baseInfo.id == id);
    }

    public static BaseSkillClass GetClass(string name)
    {
        return classes.Find(resultClass => resultClass.baseInfo.name == name);
    }
}
