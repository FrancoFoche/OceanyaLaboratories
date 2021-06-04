using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    public BaseSkillClass rpgClass;
    public PlayerCharacter(int ID, string name, int level, BaseSkillClass rpgClass, Dictionary<Stats, int> stats, List<Skill> skillList, Dictionary<Item, int> inventory)
    {
        InitializeVariables();

        this.ID = ID;
        this.level = level;
        this.name = name;
        this.rpgClass = rpgClass;
        this.stats = stats;
        this.inventory = ConvertItemsToItemInfo(inventory);
        this.skillList = ConvertSkillsToSkillInfo(skillList);
    }
}
