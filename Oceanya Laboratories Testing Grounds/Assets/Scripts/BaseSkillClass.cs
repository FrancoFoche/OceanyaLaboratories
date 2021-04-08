using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSkillClass
{
    public BaseObjectInfo baseInfo;
    public List<Skill> skillList;
    public Dictionary<string, int> statBoosts = new Dictionary<string, int>();

    public BaseSkillClass(BaseObjectInfo baseInfo, List<Skill> skillList)
    {
        this.baseInfo = baseInfo;
        this.skillList = skillList;
    }

    public BaseSkillClass(BaseObjectInfo baseInfo, List<Skill> skillList, Dictionary<string, int> statBoosts)
    {
        this.baseInfo = baseInfo;
        this.skillList = skillList;
        this.statBoosts = statBoosts;
    }
}
