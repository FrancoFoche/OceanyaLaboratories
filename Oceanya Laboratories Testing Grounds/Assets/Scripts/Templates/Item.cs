using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Activatables
{
    public enum Type
    {
        Consumable,
        Equippable
    }

    public Type type;
    public Sprite icon;

    #region Constructors
    public Item(BaseObjectInfo baseInfo, string activationText, Type type, Sprite icon, ActivatableType skillType, TargetType targetType, int maxTargets = 1)
    {
        this.name = baseInfo.name;
        ID = baseInfo.id;
        description = baseInfo.description;
        this.activationText = activationText;
        this.targetType = targetType;
        this.maxTargets = maxTargets;
        activatableType = skillType;
        this.type = type;
        this.icon = icon;
        //Default and initializer values go here

        cooldown = 0;
        behaviors = new List<Behaviors>();
    }
    public Item BehaviorDoesDamage(DamageType damageType, ElementType damageElement, List<RPGFormula> damageFormula)
    {
        behaviors.Add(Behaviors.DoesDamage);
        doesDamage = true;
        this.damageType = damageType;
        this.damageElement = damageElement;
        this.damageFormula = damageFormula;
        return this;
    }
    public Item BehaviorHasCooldown(CDType cdType)
    {
        behaviors.Add(Behaviors.HasCooldown);
        this.cdType = cdType;
        return this;
    }
    public Item BehaviorHasCooldown(CDType cdType, int cooldown)
    {
        behaviors.Add(Behaviors.HasCooldown);
        this.cdType = cdType;
        this.cooldown = cooldown;
        return this;
    }
    public Item BehaviorDoesHeal(List<RPGFormula> healFormula)
    {
        behaviors.Add(Behaviors.DoesHeal);
        doesHeal = true;
        this.healFormula = healFormula;
        return this;
    }
    public Item BehaviorModifiesStat(StatModificationTypes modificationType, Dictionary<Stats, int> statModifiers)
    {
        behaviors.Add(Behaviors.FlatModifiesStat);
        flatModifiesStat = true;
        flatStatModifiers = statModifiers;
        this.modificationType = modificationType;
        return this;
    }
    public Item BehaviorModifiesStat(StatModificationTypes modificationType, Dictionary<Stats, List<RPGFormula>> statModifiers)
    {
        behaviors.Add(Behaviors.FormulaModifiesStat);
        formulaModifiesStat = true;
        formulaStatModifiers = statModifiers;
        this.modificationType = modificationType;
        return this;
    }
    public Item BehaviorModifiesResource(Dictionary<SkillResources, int> resourceModifiers)
    {
        behaviors.Add(Behaviors.ModifiesResource);
        modifiesResource = true;
        this.resourceModifiers = resourceModifiers;
        return this;
    }
    public Item BehaviorUnlocksResource(List<SkillResources> unlockedResources)
    {
        behaviors.Add(Behaviors.UnlocksResource);
        unlocksResource = true;
        this.unlockedResources = unlockedResources;
        return this;
    }
    public Item BehaviorPassive(ActivationTime activationType)
    {
        behaviors.Add(Behaviors.Passive);
        hasPassive = true;
        this.passiveActivationType = activationType;
        return this;
    }
    public Item BehaviorCostsTurn()
    {
        behaviors.Add(Behaviors.CostsTurn);
        costsTurn = true;
        return this;
    }
    public Item BehaviorActivationRequirement(List<ActivationRequirement> requirements)
    {
        behaviors.Add(Behaviors.ActivationRequirement);
        hasActivationRequirement = true;
        activationRequirements = requirements;
        return this;
    }
    public Item BehaviorLastsFor(int maxActivationTimes)
    {
        behaviors.Add(Behaviors.LastsFor);
        lasts = true;
        lastsFor = maxActivationTimes;
        return this;
    }
    public Item BehaviorChangesBasicAttack(List<RPGFormula> newBaseFormula, DamageType newDamageType)
    {
        behaviors.Add(Behaviors.ChangesBasicAttack);
        this.newBasicAttackFormula = newBaseFormula;
        this.newBasicAttackDamageType = newDamageType;
        changesBasicAttack = true;
        return this;
    }
    public Item BehaviorRevives()
    {
        behaviors.Add(Behaviors.Revives);
        revives = true;
        return this;
    }

    #endregion

    public override void Activate(Character caster)
    {
        ItemInfo info = caster.GetItemFromInventory(this);

        info.CheckActivatable();

        if (info.activatable)
        {
            bool firstActivation = !info.currentlyActive;

            if (activatableType == ActivatableType.Active && firstActivation && hasPassive)
            {
                BattleManager.i.battleLog.LogBattleEffect($"The passive of {name} was activated for {caster.name}.");
                info.SetActive();

                if (costsTurn)
                {
                    TeamOrderManager.EndTurn();
                }
            }
            else
            {
                if (targetType == TargetType.Single || targetType == TargetType.Multiple)
                {
                    UICharacterActions.instance.maxTargets = maxTargets;
                    UICharacterActions.instance.ActionRequiresTarget(CharActions.Item);
                }
                else
                {
                    List<Character> targets = new List<Character>();
                    switch (targetType)
                    {
                        case TargetType.Self:
                            targets = new List<Character>() { caster };
                            break;

                        case TargetType.AllAllies:
                            if (caster.team == Team.Ally)
                            {
                                targets = TeamOrderManager.allySide;
                            }
                            else
                            {
                                targets = TeamOrderManager.enemySide;
                            }
                            break;
                        case TargetType.AllEnemies:
                            if (caster.team == Team.Ally)
                            {
                                targets = TeamOrderManager.enemySide;
                            }
                            else
                            {
                                targets = TeamOrderManager.allySide;
                            }
                            break;
                        case TargetType.Bounce:
                            targets = new List<Character>() { BattleManager.caster };
                            break;
                    }

                    BattleManager.i.SetTargets(targets);

                    Action(caster, targets);
                }
            }
        }
        else
        {
            BattleManager.i.battleLog.LogBattleEffect($"But {caster.name} did not meet the requirements to activate the skill!");
        }
    }

    public override void Action(Character caster, List<Character> target)
    {
        ItemInfo info = caster.GetItemFromInventory(this);

        bool firstActivation = !info.currentlyActive;

        if ((targetType == TargetType.Single || targetType == TargetType.Multiple) && firstActivation)
        {
            info.SetActive();
        }

        for (int i = 0; i < target.Count; i++)
        {
            Dictionary<ReplaceStringVariables, string> activationText = new Dictionary<ReplaceStringVariables, string>();

            activationText.Add(ReplaceStringVariables._caster_, caster.name);
            activationText.Add(ReplaceStringVariables._target_, target[i].name);

            int tempDmg = 0;
            bool wasDefending = false;
            if (target[i].targettable)
            {
                if (doesDamage)
                {
                    int rawDMG = RPGFormula.ReadAndSumList(damageFormula, caster.stats);

                    int finalDMG = target[i].CalculateDefenses(rawDMG, damageType);
                    tempDmg = finalDMG;
                    if (target[i].defending)
                    {
                        wasDefending = true;
                    }

                    target[i].GetsDamagedBy(finalDMG);

                    activationText.Add(ReplaceStringVariables._damage_, finalDMG.ToString());
                }
                if (doesHeal)
                {
                    int healAmount = RPGFormula.ReadAndSumList(healFormula, caster.stats);

                    target[i].GetsHealedBy(healAmount);

                    activationText.Add(ReplaceStringVariables._heal_, healAmount.ToString());
                }
                if (flatModifiesStat)
                {
                    target[i].ModifyStat(modificationType, flatStatModifiers);
                }
                if (formulaModifiesStat)
                {
                    Dictionary<Stats, int> resultModifiers = new Dictionary<Stats, int>();
                    for (int j = 0; j < RuleManager.StatHelper.Length; j++)
                    {
                        Stats currentStat = RuleManager.StatHelper[j];

                        if (formulaStatModifiers.ContainsKey(currentStat))
                        {
                            resultModifiers.Add(currentStat, RPGFormula.ReadAndSumList(formulaStatModifiers[currentStat], caster.stats));
                        }
                    }

                    target[i].ModifyStat(modificationType, resultModifiers);
                }
                if (unlocksResource)
                {
                    target[i].UnlockResources(unlockedResources);
                }
                if (modifiesResource)
                {
                    target[i].ModifyResource(resourceModifiers);
                }
                if (changesBasicAttack)
                {
                    target[i].ChangeBaseAttack(newBasicAttackFormula, newBasicAttackDamageType);
                }
                if (revives)
                {
                    target[i].Revive();
                }

                if (appliesStatusEffects)
                {

                }

                if (doesSummon)
                {

                }

                if (doesShield)
                {

                }
            }
            else
            {
                BattleManager.i.battleLog.LogBattleEffect("Target wasn't targettable, smh");
            }

            BattleManager.i.battleLog.LogBattleEffect(ReplaceActivationText(activationText));

            if (wasDefending && doesDamage)
            {
                BattleManager.i.battleLog.LogBattleEffect($"But {target[i].name} was defending! Meaning they actually just took {Mathf.Floor(tempDmg / 2)} DMG.");
            }

        }


        if (activatableType != ActivatableType.Passive && !hasPassive)
        {
            caster.GetItemFromInventory(this).SetDeactivated();
        }

        if (costsTurn)
        {
            if (!(activatableType == ActivatableType.Active && hasPassive && caster.GetItemFromInventory(this).currentlyActive))
            {
                TeamOrderManager.EndTurn();
            }
        }
    }
}

public class ItemInfo : ActivatableInfo
{
    public Item item                    { get; private set; }
    public int amount                   { get; private set; }

    public ItemInfo(Character character, Item item, int amount)
    {
        this.character = character;
        this.item = item;
        this.amount = amount;
        activatable = true;
    }

    public void CheckActivatable()
    {
        activatable = ActivatableInfo.CheckActivatable(item);
    }
    public override void SetDeactivated()
    {
        base.SetDeactivated();
        if (item.hasPassive)
        {
            BattleManager.i.battleLog.LogBattleEffect($"{item.name} deactivated for {character.name}.");
        }

        switch (item.type)
        {
            case Item.Type.Consumable:
                amount -= 1;
                if (amount <= 0)
                {
                    character.inventory.Remove(this);
                }
                break;
            case Item.Type.Equippable:
                Unequip();
                break;
        }
    }
    public override void SetActive()
    {
        base.SetActive();

        switch (item.type)
        {
            case Item.Type.Consumable:
                break;
            case Item.Type.Equippable:
                Equip();
                
                break;
        }
    }
    public void SetItem(Item item)
    {
        this.item = item;
    }
    public void SetAmount(int amount)
    {
        this.amount = amount;
    }
}