using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBSkills : MonoBehaviour
{
    public static List<Skill> skills = new List<Skill>();

    public static void BuildDatabase()
    {

        for (int i = 0; i < DBClasses.classes.Count; i++)
        {
            BaseSkillClass currentClass = DBClasses.GetClass(i);

            for (int j = 0; j < currentClass.skillList.Count ; j++)
            {
                Skill currentSkill = currentClass.skillList[j];
                currentSkill.skillClass = DBClasses.GetClass(i);
                skills.Add(currentSkill);
            }
        }
    }

    public static Skill GetSkill(int classID, int skillID)
    {
        return skills.Find(resultSkill => resultSkill.baseInfo.id == skillID && resultSkill.skillClass.baseInfo.id == classID);
    }

    public static Skill GetSkill(string className, string skillName)
    {
        return skills.Find(resultSkill => resultSkill.baseInfo.name == skillName && resultSkill.skillClass.baseInfo.name == className);
    }

    public static void ReadSkill(Skill skill)
    {
        print("Name: " + skill.baseInfo.name);
        print("ID: " + skill.baseInfo.id);
        print("Description: " + skill.baseInfo.description);
        print("SkillType: " + skill.type);
        print("Class: " + skill.skillClass.baseInfo.name);

        if (skill.doesDamage)
        {
            print("Skill does damage, damage properties: ");
            print("Type: " + skill.damageType);
            print("Element: " + skill.damageElement);
            print("Damage Formula: " + SkillFormula.FormulaListToString(skill.damageFormula));
        }

        if (skill.doesHeal)
        {

        }

        if (skill.flatModifiesStat)
        {

        }

        if (skill.formulaModifiesStat)
        {

        }

        if (skill.modifiesResource)
        {

        }

        if (skill.costsTurn)
        {

        }

        if (skill.appliesStatusEffects)
        {

        }

        if (skill.doesSummon)
        {

        }

        if (skill.doesShield)
        {

        }
    }

    public static void ReadDatabase()
    {
        for (int i = 0; i < skills.Count; i++)
        {
            print("SKILL NUMBER " + (i + 1) + "!");

            ReadSkill(skills[i]);

            print("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        }
    }
}
