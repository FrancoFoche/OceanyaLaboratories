using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

            new BaseSkillClass(new BaseObjectInfo(ClassNames.Nobody.ToString(), 0 , ""),
                new List<Skill>
                {

                }
            ),

            new BaseSkillClass(new BaseObjectInfo(ClassNames.SenjutsuMastery.ToString(), 1, ""),
                new List<Skill>
                {
                    new Skill
                    (
                            new BaseObjectInfo("Unlock Nature Energy", 0 , "Unlocks NATURE ENERGY resource."),
                            SkillType.Passive
                    )
                    .BehaviorUnlocksResource(new List<SkillResources>{SkillResources.NatureEnergy}),

                    new Skill
                    (
                            new BaseObjectInfo("Gather Nature Energy", 1 , "Your body has gotten used to gathering Nature Energy, you can now stay focused and gather Nature Energy! Even in combat! (Each use of Gather Nature Energy charges 1 Nature Energy Point) (Costs Turn)"),
                            SkillType.Active
                    )
                    .BehaviorCostsTurn()
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , 1 }
                        }
                    ),

                    new Skill
                    (
                            new BaseObjectInfo("Weak Sage Mode", 2 , "Your body has still not found balance between the chakras and your body has partially turned into a frog. Still better than before though? +10%STR +10%HP +5%HP Regeneration (Nature Energy cost: 1)"),
                            SkillType.Active
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
                            {Stats.STR , new SkillFormula(Stats.STR, SkillFormula.operationActions.Multiply, 0.1f) },
                            {Stats.MAXHP , new SkillFormula(Stats.MAXHP, SkillFormula.operationActions.Multiply, 0.1f) },
                            {Stats.HPREGEN , new SkillFormula(Stats.HPREGEN, SkillFormula.operationActions.Multiply, 0.05f) }
                        }
                    ),

                    new Skill
                    (
                            new BaseObjectInfo("Imperfect Sage Mode", 4 , "Your body has found a better balance between the chakras and you only have SOME frog features. Definitely better than before. +15%STR +15%HP +10%HP Regeneration (Nature Energy cost: 1)"),
                            SkillType.Active
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
                            {Stats.STR , new SkillFormula(Stats.STR, SkillFormula.operationActions.Multiply, 0.15f) },
                            {Stats.MAXHP , new SkillFormula(Stats.MAXHP, SkillFormula.operationActions.Multiply, 0.15f) },
                            {Stats.HPREGEN , new SkillFormula(Stats.HPREGEN, SkillFormula.operationActions.Multiply, 0.1f) }
                        }
                    ),

                    new Skill//Make a status effect system.
                    (
                            new BaseObjectInfo("Wood Style! Sage Mode!", 5 , "Your body knows balance! You're as beautiful as ever! You now have access to Wood Sage Arts! Your skills will be heavily based on defense! I guess you got a skill TREE. Get it? Cuz wood. and trees? whatever just have your stats .+20%STR +50%HP +25%HP Regeneration (Lasts 6 Turns) (Nature Energy cost: 2)"),
                            SkillType.Active
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
                            {Stats.STR , new SkillFormula(Stats.STR, SkillFormula.operationActions.Multiply, 0.2f) },
                            {Stats.MAXHP , new SkillFormula(Stats.MAXHP, SkillFormula.operationActions.Multiply, 0.5f) },
                            {Stats.HPREGEN , new SkillFormula(Stats.HPREGEN, SkillFormula.operationActions.Multiply, 0.25f) }
                        }
                    ),

                    new Skill//Make a passive system.
                    (
                            new BaseObjectInfo("Hair Protection", 6 , "Use your Sage Chakra to turn your hair into a protective layer of spikes! Anyone who attacks you will receive 25% of your max HP as Physical Damage (Effect is also applied to Wood Clones) (Sage Mode Turn cost: 2)"),
                            SkillType.Active
                    )
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -2 }
                        }
                    )
                    .BehaviorDoesDamage(
                        DamageType.Physical, ElementType.Normal, new List<SkillFormula>(){ new SkillFormula(Stats.MAXHP, SkillFormula.operationActions.Multiply, 0.25f) }
                    ),

                    new Skill
                    (
                        new BaseObjectInfo("Great Fire Ball", 10 , "Kinda self explanatory isn't it? Does 200% STR btw (Sage Mode Turn cost: 3)"), //Name, ID, description
                        SkillType.Active
                    )
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Magical, ElementType.Fire,
                         new List<SkillFormula>()
                         {
                            new SkillFormula(Stats.STR , SkillFormula.operationActions.Multiply , 2)
                         }),
                }
            )
        };
    }

    public enum ClassNames
    {
        Nobody,
        SenjutsuMastery,
        DojutsuMastery,
        FrostGiant,
        MartialArtist,
        Assassin,
        MasterOfDarkArts,
        Gunslinger,
        Technician,
        Vampire,
        Doctor,
        PuppetMaster,
        Ninja,
        MonsterHunter
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
