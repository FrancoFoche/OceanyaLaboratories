using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Template for any skill that is created
/// </summary>
public class Skill
{
    /// <summary>
    /// This is just for testing purposes, any skill that has this boolean as "true" means that it currently works as intended. You can get all skills that are done through the skill database function "GetAllDoneSkills"
    /// </summary>
    public bool done = false; 

    public BaseObjectInfo baseInfo { get; private set; }
    public BaseSkillClass skillClass; //RPG Class it's from
    public SkillType skillType;

    public string activationText { get; private set; }

    public bool hasActivationRequirement;
    public List<ActivationRequirement> activationRequirements;

    public bool hasPassive;
    public ActivationTime passiveActivationType { get; private set; }

    public TargetType targetType; //If the skill targets anyone, what is its target type?
    public int maxTargets;

    public CDType cdType;
    public int cooldown = 0;

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
    public Skill(BaseObjectInfo baseInfo, string activationText, SkillType skillType, TargetType targetType, int maxTargets = 1)
    {
        this.baseInfo = baseInfo;
        this.activationText = activationText;
        this.targetType = targetType;
        this.maxTargets = maxTargets;
        this.skillType = skillType;
    }
    public Skill BehaviorDoesDamage(DamageType damageType, ElementType damageElement, List<SkillFormula> damageFormula)
    {
        doesDamage = true;
        this.damageType = damageType;
        this.damageElement = damageElement;
        this.damageFormula = damageFormula;
        return this;
    }
    public Skill BehaviorHasCooldown(CDType cdType, int cooldown)
    {
        this.cdType = cdType;
        this.cooldown = cooldown;
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
    public Skill BehaviorPassive(ActivationTime activationType)
    {
        hasPassive = true;
        this.passiveActivationType = activationType;
        return this;
    }
    public Skill BehaviorCostsTurn()
    {
        costsTurn = true;
        return this;
    }
    public Skill BehaviorActivationRequirement(List<ActivationRequirement> requirements)
    {
        hasActivationRequirement = true;
        activationRequirements = requirements;
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
    
    /// <summary>
    /// just marks the skill as done, this is just for development purposes
    /// </summary>
    /// <returns></returns>
    public Skill IsDone()
    {
        done = true;
        return this;
    }
    #endregion

    public void Activate(Character caster)
    {
        SkillInfo skillInfo = caster.GetSkillFromSkillList(this);

        skillInfo.CheckActivatable();

        if (skillInfo.activatable)
        {
            bool firstActivation = !skillInfo.wasActivated;
            skillInfo.SetActive();

            if (skillType == SkillType.Active && firstActivation && hasPassive)
            {
                BattleManager.battleLog.LogBattleEffect($"The passive of {baseInfo.name} was activated for {caster.name}.");

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
                        SkillAction(caster, new List<Character>() { caster });
                        break;

                    case TargetType.Single:
                    case TargetType.Multiple:
                        UICharacterActions.instance.maxTargets = maxTargets;
                        UICharacterActions.instance.ActionRequiresTarget(CharActions.Skill);
                        break;

                    case TargetType.AllAllies:
                        if (caster.team == Team.Ally)
                        {
                            SkillAction(caster, TeamOrderManager.allySide);
                        }
                        else
                        {
                            SkillAction(caster, TeamOrderManager.enemySide);
                        }
                        break;
                    case TargetType.AllEnemies:
                        if (caster.team == Team.Ally)
                        {
                            SkillAction(caster, TeamOrderManager.enemySide);
                        }
                        else
                        {
                            SkillAction(caster, TeamOrderManager.allySide);
                        }
                        break;
                    case TargetType.Bounce:
                        SkillAction(caster, new List<Character>() { BattleManager.caster });
                        break;
                }
            }


            if (skillType != SkillType.Passive && !hasPassive)
            {
                skillInfo.SetDeactivated();
            }
        }
        else
        {
            BattleManager.battleLog.LogBattleEffect($"But {caster.name} did not meet the requirements to activate the skill!");
        }
    }

    public void SkillAction(Character caster, List<Character> target)
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
                    int rawDMG = SkillFormula.ReadAndSumList(damageFormula, caster.stats);

                    int finalDMG = target[i].CalculateDefenses(rawDMG, damageType);

                    target[i].GetsDamagedBy(finalDMG);

                    activationText.Add(ReplaceStringVariables._damage_, finalDMG.ToString());
                }
                if (doesHeal)
                {
                    int healAmount = SkillFormula.ReadAndSumList(healFormula, caster.stats);

                    target[i].GetsHealedBy(healAmount);

                    activationText.Add(ReplaceStringVariables._heal_, healAmount.ToString());
                }
                if (flatModifiesStat)
                {
                    target[i].ModifyStat(flatStatModifiers);
                }
                if (formulaModifiesStat)
                {
                    Dictionary<Stats, int> resultModifiers = new Dictionary<Stats, int>();
                    for (int j = 0; j < RuleManager.StatHelper.Length; j++)
                    {
                        Stats currentStat = RuleManager.StatHelper[j];

                        if (formulaStatModifiers.ContainsKey(currentStat))
                        {
                            resultModifiers.Add(currentStat, SkillFormula.Read(formulaStatModifiers[currentStat], caster.stats));
                        }
                    }

                    target[i].ModifyStat(resultModifiers);
                }
                if (unlocksResource)
                {
                    target[i].UnlockResources(unlockedResources);
                }
                if (modifiesResource)
                {
                    target[i].ModifyResource(resourceModifiers);
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
                BattleManager.battleLog.LogBattleEffect("Target wasn't targettable, smh");
            }

            BattleManager.battleLog.LogBattleEffect(ReplaceActivationText(activationText));

            BattleManager.UpdateUIs();
        }

        if (costsTurn)
        {
            if (!(skillType == SkillType.Active && hasPassive && caster.GetSkillFromSkillList(this).currentlyActive))
            {
                TeamOrderManager.EndTurn();
            }
        }
    }

    public string ReplaceActivationText(Dictionary<ReplaceStringVariables, string> replaceWith)
    {
        string resultText = activationText;

        for (int i = 0; i < RuleManager.ReplaceableStringsHelper.Length; i++)
        {
            ReplaceStringVariables curVariable = RuleManager.ReplaceableStringsHelper[i];
            string curVarString = curVariable.ToString();

            bool dictionaryHasVariable = replaceWith.ContainsKey(curVariable);
            bool textHasVariable = resultText.Contains(curVarString);

            if (dictionaryHasVariable && textHasVariable)
            {
                resultText = resultText.Replace(curVarString, replaceWith[curVariable]);
            }
        }

        return resultText;
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


    public static int       ReadAndSumList      (List<SkillFormula> formulas, Dictionary<Stats, int> stats)     
    {
        int result = 0;

        for (int i = 0; i < formulas.Count; i++)
        {
            result += Read(formulas[i] , stats);
        }

        return result;
    }

    public static int       Read                (SkillFormula skillFormula, Dictionary<Stats, int> stats)       
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

    public static string    FormulaToString     (SkillFormula skillFormula)                                     
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

    public static string    FormulaListToString (List<SkillFormula> skillFormulas)                              
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


/// <summary>
/// The individual skill information that is specific to a character. (CurrentCD, when a player activated the skill, etc)
/// </summary>
public class SkillInfo
{
            Character       character;
    public  Skill           skill               { get; private set; }
    public  bool            activatable         { get; private set; }
    public  bool            equipped            { get; private set; }
    public  bool            wasActivated        { get; private set; } //If the skill was activated at SOME point.
    public  bool            currentlyActive     { get; private set; } //If the skill is currently active
    public  int             activatedAt         { get; private set; } //when the skill was activated
    public  CooldownStates  cooldownState       { get; private set; }
    public  int             currentCooldown     { get; private set; }

    public SkillInfo            (Character character, Skill skill)  
    {
        this.character = character;
        this.skill = skill;
        equipped = true;
        activatable = true;
    }
    public void SetSkill        (Skill skill)                       
    {
        this.skill = skill;
    }
    public void Equip           ()                                  
    {
        equipped = true;
    }
    public void Unequip         ()                                  
    {
        equipped = false;
    }
    public void SetActive       ()                                  
    {
        currentlyActive = true;
        wasActivated = true;
        activatedAt = character.timesPlayed;
        UpdateCD();
    }
    public void SetDeactivated  ()                                  
    {
        currentlyActive = false;
        UpdateCD();
    }
    public void UpdateCD        ()                                  
    {
        CooldownStates newState = CooldownStates.Usable;

        if (wasActivated)
        {
            int difference = character.timesPlayed - activatedAt;
            currentCooldown = skill.cooldown - difference + 1;

            if (difference == 0)
            {
                newState = CooldownStates.BeingUsed;
            }
            else
            {
                switch (skill.cdType)
                {
                    case CDType.Turns:

                        if (difference <= skill.cooldown)
                        {
                            newState = CooldownStates.OnCooldown;
                        }
                        else if (difference > skill.cooldown)
                        {
                            newState = CooldownStates.Usable;
                        }

                        break;

                    case CDType.Other:
                        newState = CooldownStates.Used;
                        break;
                }
            }
        }

        cooldownState = newState;
    }
    public void CheckActivatable()                                  
    {
        bool activatable = true;

        if (skill.hasActivationRequirement)
        {
            for (int i = 0; i < skill.activationRequirements.Count; i++)
            {
                if (!skill.activationRequirements[i].CheckRequirement())
                {
                    activatable = false;
                }
            }

        }

        this.activatable = activatable;
    }
}

public class ActivationRequirement
{
    public  RequirementType type        { get; private set; }

    public  SkillResources  resource    { get; private set; }
    public  Stats           stat        { get; private set; }
    //add a status one here whenever it's done
            int             skillclassID;
            int             skillID;
    public  Skill           skill       { get; private set; }
    public  ComparerType    comparer    { get; private set; }
    public  int             number      { get; private set; }

    public  enum RequirementType
    {
        Stat,
        Resource,
        Status,
        SkillIsActive
    }
    public  enum ComparerType
    {
        MoreThan,
        LessThan,
        Equal
    }

    #region Constructors
    /// <summary>
    /// StatRequirement
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="comparingType"></param>
    /// <param name="number"></param>
    public ActivationRequirement(Stats stat, ComparerType comparingType, int number)
    {
        type = RequirementType.Stat;
        this.stat = stat;
        comparer = comparingType;
        this.number = number;
    }
    /// <summary>
    /// Resource requirement
    /// </summary>
    /// <param name="resource"></param>
    /// <param name="comparingType"></param>
    /// <param name="number"></param>
    public ActivationRequirement(SkillResources resource, ComparerType comparingType, int number)
    {
        type = RequirementType.Resource;
        this.resource = resource;
        comparer = comparingType;
        this.number = number;
    }
    /// <summary>
    /// SkillIsActive requirement.
    /// </summary>
    /// <param name="skillclassID"></param>
    /// <param name="skillID"></param>
    public ActivationRequirement(int skillclassID, int skillID)
    {
        type = RequirementType.SkillIsActive;
        this.skillclassID = skillclassID;
        this.skillID = skillID;
    }
    #endregion

    public  bool CheckRequirement()             
    {
        Character caster = BattleManager.caster;

        switch (type)
        {
            case RequirementType.Stat:
                return CheckRequirement(caster.stats[stat]);
                
            case RequirementType.Resource:
                return CheckRequirement(caster.skillResources[resource]);

            case RequirementType.Status:
                Debug.LogError("Requirement type status not yet implemented, returning true");
                return true;

            case RequirementType.SkillIsActive:
                if(skill == null)
                {
                    skill = DBSkills.GetSkill(skillclassID, skillID);
                }
                return caster.GetSkillFromSkillList(skill).currentlyActive;

            default:
                Debug.LogError("Invalid Requirement type, returning true");
                return true;
        }
    }
    public  bool CheckRequirement(int number)   
    {
        switch (comparer)
        {
            case ComparerType.MoreThan:
                return number > this.number;

            case ComparerType.LessThan:
                return number < this.number;

            case ComparerType.Equal:
                return number == this.number;

            default:
                Debug.LogError("Invalid Comparer type, returning true");
                return true;
        }
    }
}
