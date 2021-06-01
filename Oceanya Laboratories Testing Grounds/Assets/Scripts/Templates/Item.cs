using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Activatables
{
    public override void Activate(Character caster)
    {
        ItemInfo itemInfo = caster.GetItemFromInventory(this);

        itemInfo.CheckActivatable();

        if (itemInfo.activatable)
        {
            bool firstActivation = !itemInfo.currentlyActive;
            itemInfo.SetActive();

            if (activatableType == ActivatableType.Active && firstActivation && hasPassive)
            {
                BattleManager.i.battleLog.LogBattleEffect($"The passive of {name} was activated for {caster.name}.");

                if (costsTurn)
                {
                    TeamOrderManager.EndTurn();
                }
            }
            else
            {
                switch (targetType)
                {
                    case TargetType.Self:
                        Action(caster, new List<Character>() { caster });
                        break;

                    case TargetType.Single:
                    case TargetType.Multiple:
                        UICharacterActions.instance.maxTargets = maxTargets;
                        UICharacterActions.instance.ActionRequiresTarget(CharActions.Skill);
                        break;

                    case TargetType.AllAllies:
                        if (caster.team == Team.Ally)
                        {
                            Action(caster, TeamOrderManager.allySide);
                        }
                        else
                        {
                            Action(caster, TeamOrderManager.enemySide);
                        }
                        break;
                    case TargetType.AllEnemies:
                        if (caster.team == Team.Ally)
                        {
                            Action(caster, TeamOrderManager.enemySide);
                        }
                        else
                        {
                            Action(caster, TeamOrderManager.allySide);
                        }
                        break;
                    case TargetType.Bounce:
                        Action(caster, new List<Character>() { BattleManager.caster });
                        break;
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

    public ItemInfo(Character character, Item item)
    {
        this.character = character;
        this.item = item;
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