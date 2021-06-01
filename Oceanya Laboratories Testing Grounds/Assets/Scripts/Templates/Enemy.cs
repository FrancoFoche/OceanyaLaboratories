using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public Sprite image;

    public Enemy(int ID, string name, Sprite image, int level, Dictionary<Stats, int> stats, List<Skill> skillList, List<Item> items)
    {
        InitializeVariables();

        this.ID = ID;
        this.level = level;
        this.name = name;
        this.stats = stats;
        this.image = image;
        this.inventory = ConvertItemsToItemInfo(items);

        this.skillList = ConvertSkillsToSkillInfo(skillList);
    }
}
