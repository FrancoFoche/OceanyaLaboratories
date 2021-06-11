using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerCharacter : Character
{
    public BaseSkillClass rpgClass;
    public PlayerCharacter(int ID, string name, int startingLevel, BaseSkillClass rpgClass, Dictionary<Stats, int> stats, List<Skill> skillList, Dictionary<Item, int> inventory)
    {
        InitializeVariables();

        this.ID = ID;
        this.level = new LevellingSystem().SetStartingLevel(startingLevel);
        this.name = name;

        this.rpgClass = rpgClass;

        _nonBuffedStats = stats;

        this.stats = rpgClass.BoostStats(_nonBuffedStats, startingLevel);

        _originalStats = MakeCopyOfStatsDictionary(this.stats);
        
        

        this.inventory = ConvertItemsToItemInfo(inventory);
        this._originalInventory = MakeCopyOfItemInfo(this.inventory);

        this.skillList = ConvertSkillsToSkillInfo(skillList);
        this._originalSkillList = MakeCopyOfSkillInfo(this.skillList);
    }

    public override void AddExp(int exp)
    {
        int oldLevel = level.Level;

        base.AddExp(exp);

        if (oldLevel != level.Level)
        {
            stats = rpgClass.BoostStats(_nonBuffedStats, level.Level);
        }
    }
}
