using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCharacter : Character
{
    public int              ID;
    public BaseSkillClass   rpgClass;
    
    public List<Skill>      activeSkillList;
    public List<Skill>      hiddenSkillList;

    public PlayerCharacter(int ID, string name, int level, BaseSkillClass rpgClass, Dictionary<Stats, int> stats, List<Skill> activeSkillList, List<Skill> hiddenSkillList)
    {
        this.ID = ID;
        this.level = level;
        this.name = name;
        this.rpgClass = rpgClass;
        this.stats = stats;
        this.activeSkillList = activeSkillList;
        this.hiddenSkillList = hiddenSkillList;
    }
}
