using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

public class Item
{
    public BaseObjectInfo baseinfo;
    public ItemType type;
    public string activationText;

    public TargetType targetType;
    public int maxTargets;

    public bool doesDamage;
    public DamageType DamageType;

    public bool doesHeal;

    public bool modifiesStats;
    public Dictionary<Stats, int> StatModifier;
    public StatModificationTypes modificationType;

    public bool modifiesResourse;
    public Dictionary<SkillResources, int> resourseModifiers;

    public bool appliesStatusEffects;

    public bool doesShield;

    public bool revives;

    public enum ItemStats
    {
        Damage,
        Heal
    }
    public Dictionary<ItemStats, int> Stats = new Dictionary<ItemStats, int>()
    {
        {ItemStats.Damage, 0 },
        {ItemStats.Heal, 0 }
    };


    public Item(BaseObjectInfo baseInfo, ItemType itemType, TargetType targetType, Dictionary<ItemStats, int> stats)
    {
        this.baseinfo = baseInfo;
        this.type = itemType;
        this.targetType = targetType;
        this.Stats = stats;
    }

    public Item BehaviorDoesDamage(DamageType damageType)
    {
        doesDamage = true;
        DamageType = damageType;
        return this;
    }

    public Item BehaviorDoesHeal()
    {
        doesHeal = true;
        return this;
    }

    public Item BEhaviorModifiesStat(Dictionary<Stats, int> statModifier)
    {
        modifiesStats = true;
        StatModifier = statModifier;
        return this;
    }

    public Item BehabiorModifiesResourse(Dictionary<SkillResources, int> Resoursemodifier)
    {
        modifiesResourse = true;
        resourseModifiers = Resoursemodifier;
        return this;
    }

    public Item BehabiorDoesShield()
    {
        doesShield = true;
        return this;
    }

    public void Activate(Character caster)
    {
        switch (targetType)
        {
            case TargetType.Self:
                ItemAction(caster, new List<Character>() { caster });
                break;

            case TargetType.Single:
            case TargetType.Multiple:
                UICharacterActions.instance.maxTargets = maxTargets;
                UICharacterActions.instance.ActionRequiresTarget(CharActions.Item);
                break;

            case TargetType.AllAllies:
                if (caster.team == Team.Ally)
                {
                    ItemAction(caster, TeamOrderManager.allySide);
                }
                else
                {
                    ItemAction(caster, TeamOrderManager.enemySide);
                }
                break;

            case TargetType.AllEnemies:
                if (caster.team == Team.Ally)
                {
                    ItemAction(caster, TeamOrderManager.enemySide);
                }
                else
                {
                    ItemAction(caster, TeamOrderManager.allySide);
                }
                break;

            case TargetType.Bounce:
                ItemAction(caster, new List<Character>() { BattleManager.caster });
                break;
        }
    }

    public void ItemAction(Character caster, List<Character> target)
    {
        for (int i = 0; i < target.Count; i++)
        {
            Dictionary<ReplaceStringVariables, string> activationText = new Dictionary<ReplaceStringVariables, string>();

            activationText.Add(ReplaceStringVariables._caster_, caster.name);
            activationText.Add(ReplaceStringVariables._target_, target[i].name);


            if (target[i].targettable)
            {
                if (doesDamage)
                {
                    target[i].GetsDamagedBy(Stats[ItemStats.Damage]);
                }
                if (doesHeal)
                {
                    target[i].GetsHealedBy(Stats[ItemStats.Heal]);
                }
                if (modifiesStats)
                {
                    target[i].ModifyStat(modificationType, StatModifier);
                }
                if (modifiesResourse)
                {
                    target[i].ModifyResource(resourseModifiers);
                }
                if (revives)
                {
                    target[i].Revive();
                }
            }
        }

        if (this.type == ItemType.consume)
        {
            caster.inventory.RemoveAt(caster.inventory.IndexOf(this));
            UIItemContext.instance.Hide();
            UIItemContext.instance.Show();
        }
    }
}

public class ItemInfo
{
    Character character;
    public Item item { get; private set; }
    public bool activatable { get; private set; }
    public bool equipped { get; private set; }

    public ItemInfo(Character character, Item item)
    {
        this.character = character;
        this.item = item;
        equipped = true;
        activatable = true;
    }
}