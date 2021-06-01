using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSkillClass
{
    [SerializeField] private string _name;
    [SerializeField] private int _ID;
    [SerializeField] private string _description;

    public List<Skill> skillList;
    public Dictionary<string, int> statBoosts = new Dictionary<string, int>();

    public string       name                { get { return _name; }         protected set { _name = value; } }
    public int          ID                  { get { return _ID; }           protected set { _ID = value; } }
    public string       description         { get { return _description; }  protected set { _description = value; } }

    public BaseSkillClass(BaseObjectInfo baseInfo, List<Skill> skillList)
    {
        name = baseInfo.name;
        ID = baseInfo.id;
        description = baseInfo.description;
        this.skillList = skillList;
    }
}
