using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public List<BaseSkillClass> classList = DBClasses.classes;

    public void SkillCheck(BaseSkillClass skillClass, string skillName)
    {
        skillClass.skillList.Find(skill => skill.baseInfo.name == skillName);
    }
}
