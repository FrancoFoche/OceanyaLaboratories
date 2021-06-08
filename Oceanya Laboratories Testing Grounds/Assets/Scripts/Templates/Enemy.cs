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
        

        this.sprite = sprite;

        this._originalStats = MakeCopyOfStatsDictionary(stats);
        this.stats = stats;
        
        this.inventory = ConvertItemsToItemInfo(inventory);
        this._originalInventory = MakeCopyOfItemInfo(this.inventory);

        this.skillList = ConvertSkillsToSkillInfo(skillList);
        this._originalSkillList = MakeCopyOfSkillInfo(this.skillList);
    }

    public Enemy(Enemy enemy)
    {
        InitializeVariables();

        #region Values
        this.ID = enemy.ID;
        this.level = enemy.level;
        this.name = enemy.name;
        #endregion

        #region References
        this.sprite = enemy.sprite;

        this._originalStats = MakeCopyOfStatsDictionary(enemy.stats);
        this.stats = MakeCopyOfStatsDictionary(enemy.stats);

        this._originalInventory = MakeCopyOfItemInfo(enemy.inventory);
        this.inventory = MakeCopyOfItemInfo(enemy.inventory);

        this._originalSkillList = MakeCopyOfSkillInfo(enemy.skillList);
        this.skillList = MakeCopyOfSkillInfo(enemy.skillList);
        #endregion
    }
}
