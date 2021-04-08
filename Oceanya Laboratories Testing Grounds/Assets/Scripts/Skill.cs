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

    public bool doesBuff; //does the skill buff any stat
    public Dictionary<Character.Stats, int> buffStats; //what stats does it buff and by how much

    public bool doesDebuff; //does the skill debuff any stat
    public Dictionary<Character.Stats, int> debuffStats; //what stats does it debuff and by how much

    public bool costsResource; //does the skill cost a resource? (Mana, Bloodstacks, HP, etc.)
    public Dictionary<Character.SkillResources, int> costs; //what does it cost and how much

    public bool addsToResource; //does the skill add to a resource
    public Dictionary<Character.SkillResources, int> addResource; //what does it add and how much

    public bool unlocksResource; //does it unlock a resource
    public Dictionary<Character.SkillResources, bool> unlockedResources; //what resources does it unlock

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
    public Skill BehaviorDoesBuff(Dictionary<Character.Stats, int> buffStats)
    {
        doesBuff = true;
        this.buffStats = buffStats;
        return this;
    }
    public Skill BehaviorDoesDebuff(Dictionary<Character.Stats, int> debuffStats)
    {
        doesDebuff = true;
        this.debuffStats = debuffStats;
        return this;
    }
    public Skill BehaviorCostsResource(Dictionary<Character.SkillResources, int> costs)
    {
        costsResource = true;
        this.costs = costs;
        return this;
    }
    public Skill BehaviorAddsResource(Dictionary<Character.SkillResources, int> addResource)
    {
        addsToResource = true;
        this.addResource = addResource;
        return this;
    }
    public Skill BehaviorUnlocksResource(Dictionary<Character.SkillResources, bool> unlockedResources)
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

    public enum SkillType
    {
        Active,
        Passive
    }
    public enum TargetType
    {
        Self,
        Single,
        MultiTarget,
        AllAllies,
        AllEnemies
    }
    public enum DamageType
    {
        Direct,
        Magical,
        Physical
    }
    public enum ElementType
    {
        Normal,
        Water,
        Fire,
        Thunder,
        Ice,
        Wind,
        Holy,
        Dark
    }
    public enum CDType
    {
        OnceABattle,
        OnceADay
    }

    public virtual void Activate(Character caster, Character target)
    {
        string activationText = "";

        activationText = $"{caster.name} activated {baseInfo.name}! Target: {target.name}!";

        if (target.targettable)
        {
            if (doesDamage)
            {

                int rawDMG = SkillFormula.ReadAndSumList(damageFormula, caster.stats);

                int finalDMG = target.CalculateDefenses(rawDMG, DamageType.Magical, target);

                target.GetsDamagedBy(finalDMG);

                activationText += $" {finalDMG} DMG!";
            }

            if(doesHeal)
            {
                int healAmount = SkillFormula.ReadAndSumList(healFormula, caster.stats);

                target.GetsHealedBy(healAmount);

                activationText += $" Heal: {healAmount} HP!";
            }

            if(doesBuff)
            {
                //create a buff function
            }

            if(doesDebuff)
            {
                //create a debuff function
            }

            if (unlocksResource)
            {
                string unlockedResourcesList = target.UnlockResources(unlockedResources);
                activationText += $" {unlockedResourcesList}";
            }

            if (costsResource)
            {

            }

            if(addsToResource)
            {
                activationText += $" {target.AddToResource(addResource)}";
            }

            if (costsTurn)
            {

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
            activationText += " Target wasn't targettable, smh";
        }

        BattleManager.battleLog.LogBattleEffect(activationText);

        BattleManager.UpdateUIs();
    }
}

public struct SkillFormula
{
    Character.Stats StatToUse { get; }
    operationActions OperationModifier { get; }
    float NumberModifier { get; } 

    public SkillFormula(Character.Stats StatToUse, operationActions OperationModifier, float NumberModifier)
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

    public static int ReadAndSumList(List<SkillFormula> formulas, Dictionary<Character.Stats, int> stats)
    {
        int result = 0;

        for (int i = 0; i < formulas.Count; i++)
        {
            result += Read(formulas[i] , stats);
        }

        return result;
    }

    public static int Read(SkillFormula skillFormula, Dictionary<Character.Stats, int> stats)
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
