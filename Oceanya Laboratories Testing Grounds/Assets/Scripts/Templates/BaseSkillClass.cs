using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSkillClass
{
    [SerializeField] private string _name;
    [SerializeField] private int _ID;
    [SerializeField] private string _description;

    public List<Skill> skillList;
    public Dictionary<Stats, int> statBoosts = new Dictionary<Stats, int>();

    public string       name                { get { return _name; }         protected set { _name = value; } }
    public int          ID                  { get { return _ID; }           protected set { _ID = value; } }
    public string       description         { get { return _description; }  protected set { _description = value; } }

    public BaseSkillClass(BaseObjectInfo baseInfo, Dictionary<Stats, int> statBoosts, List<Skill> skillList)
    {
        name = baseInfo.name;
        ID = baseInfo.id;
        description = baseInfo.description;
        this.statBoosts = statBoosts;
        this.skillList = skillList;
    }

    public Dictionary<Stats, int> BoostStats(Dictionary<Stats, int> originalStats, int level)
    {
        Dictionary<Stats, int> boostedStats = new Dictionary<Stats, int>();

        foreach(var kvp in originalStats)
        {
            if (statBoosts.ContainsKey(kvp.Key))
            {
                boostedStats.Add(kvp.Key, kvp.Value + (statBoosts[kvp.Key] * level) + level);
            }
            else
            {
                boostedStats.Add(kvp.Key, kvp.Value + level);
            }
        }

        return boostedStats;
    }
}
