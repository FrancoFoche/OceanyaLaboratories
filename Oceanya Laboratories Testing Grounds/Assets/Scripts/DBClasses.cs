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
                            Skill.SkillType.Passive
                    )
                    .BehaviorUnlocksResource(
                        new Dictionary<Character.SkillResources, bool>()
                        { 
                            {Character.SkillResources.NatureEnergy , true },
                            { Character.SkillResources.Mana , false },
                            { Character.SkillResources.Bloodstacks , false },
                            { Character.SkillResources.Puppets , false },
                            { Character.SkillResources.other , false },
                        }
                    ),

                    new Skill
                    (
                            new BaseObjectInfo("Gather Nature Energy", 1 , "Your body has gotten used to gathering Nature Energy, you can now stay focused and gather Nature Energy! Even in combat! (Each use of Gather Nature Energy charges 1 Nature Energy Point) (Costs Turn)"),
                            Skill.SkillType.Active
                    )
                    .BehaviorCostsTurn()
                    .BehaviorAddsResource(
                        new Dictionary<Character.SkillResources, int>()
                        {
                            { Character.SkillResources.NatureEnergy , 1 },
                            { Character.SkillResources.Mana , 0 },
                            { Character.SkillResources.Bloodstacks , 0 },
                            { Character.SkillResources.Puppets , 0 },
                            { Character.SkillResources.other , 0 },
                        }
                    ),

                    new Skill
                    (
                        new BaseObjectInfo("Great Fire Ball", 10 , "Kinda self explanatory isn't it? Does 200% STR btw (Sage Mode Turn cost: 3)"), //Name, ID, description
                        Skill.SkillType.Active //It's an active skill
                    )
                    .BehaviorCostsTurn() //it ends your turn when you use it
                    .BehaviorDoesDamage(Skill.DamageType.Magical, Skill.ElementType.Fire,
                         new List<SkillFormula>()
                         {
                            new SkillFormula(Character.Stats.STR , SkillFormula.operationActions.Multiply , 2)
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
