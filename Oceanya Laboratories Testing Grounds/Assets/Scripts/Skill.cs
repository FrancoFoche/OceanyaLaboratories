using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Template for any skill that is created
/// </summary>
public class Skill
{
    public BaseObjectInfo baseInfo;
    public SkillType type; //Skill type
    public BaseSkillClass skillClass; //RPG Class it's from

    public bool requiresTarget; //Does the skill target anyone
    public TargetType targetType; //If the skill targets anyone, what is its target type?

    public bool specialCD; //if the cooldown type is a non-turn based CD
    public CDType cooldownType; //Turn Cooldown, Once a day, once a battle

    public int turnCooldown; //in case there is a turn cooldown, set the number here

    public bool doesDamage; //If the skill does damage
    public DamageType damageType; //What type of damage does it do
    public ElementType damageElement; //What elemental type is the damage skill
    public List<SkillFormula> damageFormula; //list of formulas to sum to get the damage number

    public bool doesHeal; //if the skill heals
    public List<SkillFormula> healFormula; //list of formulas to sum to get the heal number

    public bool flatModifiesStat; //does the skill buff any stat by a flat number
    public Dictionary<Stats, int> flatStatModifiers;
    public bool formulaModifiesStat; //does the skill buff any stat by a formula
    public Dictionary<Stats, SkillFormula> formulaStatModifiers;

    public bool modifiesResource; //does the skill modify a resource? (Mana, Bloodstacks, HP, etc.)
    public Dictionary<SkillResources, int> resourceModifiers; //what does it modify and by how much

    public bool unlocksResource; //does it unlock a resource
    public List<SkillResources> unlockedResources; //what resources does it unlock

    public bool costsTurn; //does the skill end your turn

    public bool appliesStatusEffects; //does the skill apply a status effect?

    public bool doesSummon; //does the skill summon anything

    public bool doesShield; //does the skill shield anything

    #region Constructors
    public Skill(BaseObjectInfo baseInfo, SkillType type)
    {
        this.baseInfo = baseInfo;
        this.type = type;
    }
    public Skill(BaseObjectInfo baseInfo, SkillType type, BaseSkillClass skillClass)
    {
        this.baseInfo = baseInfo;
        this.type = type;
        this.skillClass = skillClass;
    }
    public Skill BehaviorRequiresTarget(TargetType targetType)
    {
        requiresTarget = true;
        this.targetType = targetType;
        return this;
    }
    public Skill BehaviorHasSpecialCooldown(CDType cooldownType)
    {
        specialCD = true;
        this.cooldownType = cooldownType;
        return this;
    }
    public Skill BehaviorDoesDamage(DamageType damageType, ElementType damageElement, List<SkillFormula> damageFormula)
    {
        doesDamage = true;
        this.damageType = damageType;
        this.damageElement = damageElement;
        this.damageFormula = damageFormula;
        return this;
    }
    public Skill BehaviorHasTurnCooldown(int turnCooldown)
    {
        specialCD = true;
        this.turnCooldown = turnCooldown;
        return this;
    }
    public Skill BehaviorDoesHeal(List<SkillFormula> healFormula)
    {
        doesHeal = true;
        this.healFormula = healFormula;
        return this;
    }
    public Skill BehaviorModifiesStat(Dictionary<Stats, int> statModifiers)
    {
        flatModifiesStat = true;
        flatStatModifiers = statModifiers;
        return this;
    }
    public Skill BehaviorModifiesStat(Dictionary<Stats, SkillFormula> statModifiers)
    {
        formulaModifiesStat = true;
        formulaStatModifiers = statModifiers;
        return this;
    }
    public Skill BehaviorModifiesResource(Dictionary<SkillResources, int> resourceModifiers)
    {
        modifiesResource = true;
        this.resourceModifiers = resourceModifiers;
        return this;
    }
    public Skill BehaviorUnlocksResource(List<SkillResources> unlockedResources)
    {
        unlocksResource = true;
        this.unlockedResources = unlockedResources;
        return this;
    }
    public Skill BehaviorCostsTurn()
    {
        costsTurn = true;
        return this;
    }
    public Skill BehaviorAppliesStatusEffects()
    {
        appliesStatusEffects = true;
        return this;
    }
    public Skill BehaviorDoesSummon()
    {
        doesSummon = true;
        return this;
    }
    public Skill BehaviorDoesShield()
    {
        doesShield = true;
        return this;
    }
    #endregion

    public virtual void Activate(Character caster, Character target)
    {
        string activationText = "";

        activationText = $"{caster.name} activated {baseInfo.name}! Target: {target.name}!";

        if (target.targettable)
        {
            if (doesDamage)//Done
            {
                int rawDMG = SkillFormula.ReadAndSumList(damageFormula, caster.stats);

                int finalDMG = target.CalculateDefenses(rawDMG, DamageType.Magical, target);

                target.GetsDamagedBy(finalDMG);

                activationText += $" {finalDMG} DMG!";
            }

            if(doesHeal)//Done
            {
                int healAmount = SkillFormula.ReadAndSumList(healFormula, caster.stats);

                target.GetsHealedBy(healAmount);

                activationText += $" Heal: {healAmount} HP!";
            }

            if(flatModifiesStat)//Done
            {
                target.ModifyStat(flatStatModifiers);
            }

            if (formulaModifiesStat) //Done
            {
                for (int i = 0; i < RuleManager.StatHelper.Count; i++)
                {
                    Stats currentStat = RuleManager.StatHelper[i];

                    if (formulaStatModifiers.ContainsKey(currentStat))
                    {
                        flatStatModifiers[currentStat] = SkillFormula.Read(formulaStatModifiers[currentStat], caster.stats);
                    }
                }

                target.ModifyStat(flatStatModifiers);
            }

            if (unlocksResource) //Done
            {
                target.UnlockResources(unlockedResources);

                string unlockedResourcesList = "";
                for (int i = 0; i < unlockedResources.Count; i++)
                {
                    unlockedResourcesList += " " + unlockedResources[i] + ";";
                }
                activationText += $" Unlocked resources: {unlockedResourcesList}";
            }

            if(modifiesResource) //Done
            {
                target.ModifyResource(resourceModifiers);
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

            if (costsTurn)
            {
                BattleManager.instance.EndTurn();
            }
        }
        else
        {
            activationText += " Target wasn't targettable, smh";
        }

        BattleManager.battleLog.LogBattleEffect(activationText);

        BattleManager.UpdateUIs();
    }
}

public struct SkillFormula
{
    Stats StatToUse { get; }
    operationActions OperationModifier { get; }
    float NumberModifier { get; } 

    public SkillFormula(Stats StatToUse, operationActions OperationModifier, float NumberModifier)
    {
        this.StatToUse = StatToUse;
        this.OperationModifier = OperationModifier;
        this.NumberModifier = NumberModifier;
    }

    public enum operationActions
    {
        Multiply,
        Divide,
        ToThePowerOf
    }

    public static int ReadAndSumList(List<SkillFormula> formulas, Dictionary<Stats, int> stats)
    {
        int result = 0;

        for (int i = 0; i < formulas.Count; i++)
        {
            result += Read(formulas[i] , stats);
        }

        return result;
    }

    public static int Read(SkillFormula skillFormula, Dictionary<Stats, int> stats)
    {
        int stat = stats[skillFormula.StatToUse];
        float number = skillFormula.NumberModifier;
        int result = 0;

        switch (skillFormula.OperationModifier)
        {
            case operationActions.Multiply:
                result = Mathf.CeilToInt(stat * number);
                break;

            case operationActions.Divide:
                result = number != 0 ? Mathf.CeilToInt(stat / number) : 0;
                break;

            case operationActions.ToThePowerOf:
                result = Mathf.CeilToInt(Mathf.Pow(stat, number));
                break;
        }

        return result;
    }

    public static string FormulaToString(SkillFormula skillFormula)
    {
        string stat = skillFormula.StatToUse.ToString();
        string operationSymbol = "";
        string number = skillFormula.NumberModifier.ToString();

        switch (skillFormula.OperationModifier)
        {
            case operationActions.Multiply:
                operationSymbol = "*";
                break;
            case operationActions.Divide:
                operationSymbol = "/";
                break;
            case operationActions.ToThePowerOf:
                operationSymbol = "to the power of";
                break;
        }

        string result = stat + " " + operationSymbol + " " + number;
        
        return result;
    }

    public static string FormulaListToString(List<SkillFormula> skillFormulas)
    {
        string result = "";

        for (int i = 0; i < skillFormulas.Count; i++)
        {
            string currentFormula = FormulaToString(skillFormulas[i]);

            if (i == 0) 
            { 
                result += currentFormula;
            }
            else
            {
                result += " + " + currentFormula;
            }
        }

        return result;
    }

}
