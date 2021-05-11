using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public Sprite image;

    public Enemy(int ID, string name, Sprite image, int level, Dictionary<Stats, int> stats, List<Skill> skillList)
    {
        InitializeVariables();

        this.ID = ID;
        this.level = level;
        this.name = name;
        this.stats = stats;
        this.image = image;

        this.skillList = ConvertSkillsToSkillInfo(skillList);
    }
}
