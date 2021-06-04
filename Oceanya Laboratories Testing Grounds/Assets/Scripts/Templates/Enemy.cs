using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public Enemy(int ID, string name, Texture2D sprite, int level, Dictionary<Stats, int> stats, List<Skill> skillList, Dictionary<Item, int> inventory)
    {
        InitializeVariables();

        this.ID = ID;
        this.level = level;
        this.name = name;
        this.stats = stats;
        this.sprite = sprite;
        this.inventory = ConvertItemsToItemInfo(inventory);

        this.skillList = ConvertSkillsToSkillInfo(skillList);
    }
}
