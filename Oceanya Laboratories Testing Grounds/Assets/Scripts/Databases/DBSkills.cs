﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBSkills : MonoBehaviour
{
    public static List<Skill> skills = new List<Skill>();

    public static void          BuildDatabase   ()                                  
    {

        for (int i = 0; i < DBClasses.classes.Count; i++)
        {
            BaseSkillClass currentClass = DBClasses.classes[i];

            for (int j = 0; j < currentClass.skillList.Count ; j++)
            {
                Skill currentSkill = currentClass.skillList[j];
                currentSkill.skillClass = currentClass;
                skills.Add(currentSkill);
            }
        }
    }

    public static Skill         GetSkill        (int classID, int skillID)          
    {
        return skills.Find(resultSkill => resultSkill.baseInfo.id == skillID && resultSkill.skillClass.baseInfo.id == classID);
    }

    public static Skill         GetSkill        (string className, string skillName)
    {
        return skills.Find(resultSkill => resultSkill.baseInfo.name == skillName && resultSkill.skillClass.baseInfo.name == className);
    }

    public static List<Skill>   GetAllSkills    ()                                  
    {
        return skills;
    }

    public static List<Skill>   GetAllDoneSkills()                                  
    {
        List<Skill> doneSkills = new List<Skill>();

        for (int i = 0; i < skills.Count; i++)
        {
            if (skills[i].done)
            {
                doneSkills.Add(skills[i]);
            }
        }
        return doneSkills;
    }
}
