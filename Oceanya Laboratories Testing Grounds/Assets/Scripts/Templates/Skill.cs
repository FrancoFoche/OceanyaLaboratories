using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Template for any skill that is created
/// </summary>
public class Skill: Activatables
{
    /// <summary>
    /// This is just for testing purposes, any skill that has this boolean as "true" means that it currently works as intended. You can get all skills that are done through the skill database function "GetAllDoneSkills"
    /// </summary>
    public bool                                     done                                { get; private set; }
    public BaseSkillClass                           skillClass;         //RPG Class it's from

    #region Constructors
    public Skill(BaseObjectInfo baseInfo, string activationText, ActivatableType skillType, TargetType targetType, int maxTargets = 1)
    {
        this.name = baseInfo.name;
        ID = baseInfo.id;
        description = baseInfo.description;
        this.activationText = activationText;
        this.targetType = targetType;
        this.maxTargets = maxTargets;
        activatableType = skillType;
        //Default and initializer values go here
        done = false;
        cooldown = 0;
        behaviors = new List<Behaviors>();
    }
    public Skill BehaviorDoesDamage(DamageType damageType, ElementType damageElement, List<RPGFormula> damageFormula)
    {
        behaviors.Add(Behaviors.DoesDamage);
        doesDamage = true;
        this.damageType = damageType;
        this.damageElement = damageElement;
        this.damageFormula = damageFormula;
        return this;
    }
    public Skill BehaviorHasCooldown(CDType cdType)
    {
        behaviors.Add(Behaviors.HasCooldown);
        this.cdType = cdType;
        return this;
    }
    public Skill BehaviorHasCooldown(CDType cdType, int cooldown)
    {
        behaviors.Add(Behaviors.HasCooldown);
        this.cdType = cdType;
        this.cooldown = cooldown;
        return this;
    }
    public Skill BehaviorDoesHeal(List<RPGFormula> healFormula)
    {
        behaviors.Add(Behaviors.DoesHeal);
        doesHeal = true;
        this.healFormula = healFormula;
        return this;
    }
    public Skill BehaviorModifiesStat(StatModificationTypes modificationType, Dictionary<Stats, int> statModifiers)
    {
        behaviors.Add(Behaviors.FlatModifiesStat);
        flatModifiesStat = true;
        flatStatModifiers = statModifiers;
        this.modificationType = modificationType;
        return this;
    }
    public Skill BehaviorModifiesStat(StatModificationTypes modificationType, Dictionary<Stats, List<RPGFormula>> statModifiers)
    {
        behaviors.Add(Behaviors.FormulaModifiesStat);
        formulaModifiesStat = true;
        formulaStatModifiers = statModifiers;
        this.modificationType = modificationType;
        return this;
    }
    public Skill BehaviorModifiesResource(Dictionary<SkillResources, int> resourceModifiers)
    {
        behaviors.Add(Behaviors.ModifiesResource);
        modifiesResource = true;
        this.resourceModifiers = resourceModifiers;
        return this;
    }
    public Skill BehaviorUnlocksResource(List<SkillResources> unlockedResources)
    {
        behaviors.Add(Behaviors.UnlocksResource);
        unlocksResource = true;
        this.unlockedResources = unlockedResources;
        return this;
    }
    public Skill BehaviorPassive(ActivationTime activationType)
    {
        behaviors.Add(Behaviors.Passive);
        hasPassive = true;
        this.passiveActivationType = activationType;
        return this;
    }
    public Skill BehaviorCostsTurn()
    {
        behaviors.Add(Behaviors.CostsTurn);
        costsTurn = true;
        return this;
    }
    public Skill BehaviorActivationRequirement(List<ActivationRequirement> requirements)
    {
        behaviors.Add(Behaviors.ActivationRequirement);
        hasActivationRequirement = true;
        activationRequirements = requirements;
        return this;
    }
    public Skill BehaviorLastsFor(int maxActivationTimes)
    {
        behaviors.Add(Behaviors.LastsFor);
        lasts = true;
        lastsFor = maxActivationTimes;
        return this;
    }
    public Skill BehaviorChangesBasicAttack(List<RPGFormula> newBaseFormula, DamageType newDamageType)
    {
        behaviors.Add(Behaviors.ChangesBasicAttack);
        this.newBasicAttackFormula = newBaseFormula;
        this.newBasicAttackDamageType = newDamageType;
        changesBasicAttack = true;
        return this;
    }
    public Skill BehaviorRevives()
    {
        behaviors.Add(Behaviors.Revives);
        revives = true;
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

    public override void Activate(Character caster)
    {
        SkillInfo skillInfo = caster.GetSkillFromSkillList(this);

        skillInfo.CheckActivatable();

        if (skillInfo.activatable)
        {
            bool firstActivation = !skillInfo.currentlyActive;

            if (activatableType == ActivatableType.Active && firstActivation && hasPassive)
            {
                BattleManager.i.battleLog.LogBattleEffect($"The passive of {name} was activated for {caster.name}.");
                skillInfo.SetActive();

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
                    UICharacterActions.instance.ActionRequiresTarget(CharActions.Skill);
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
        SkillInfo skillInfo = caster.GetSkillFromSkillList(this);

        bool firstActivation = !skillInfo.currentlyActive;

        if((targetType == TargetType.Single || targetType == TargetType.Multiple) && firstActivation)
        {
            skillInfo.SetActive();
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
                    if (target[i].dead)
                    {
                        target[i].Revive();
                    }
                    else
                    {
                        BattleManager.i.battleLog.LogBattleEffect($"But {target[i].name} was not dead...");
                    }
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
            caster.GetSkillFromSkillList(this).SetDeactivated();
        }

        if (costsTurn)
        {
            if (!(activatableType == ActivatableType.Active && hasPassive && caster.GetSkillFromSkillList(this).currentlyActive))
            {
                TeamOrderManager.EndTurn();
            }
        }
    }
}


/// <summary>
/// The individual skill information that is specific to a character. (CurrentCD, when a player activated the skill, etc)
/// </summary>
public class SkillInfo : ActivatableInfo
{
    public  Skill           skill               { get; private set; }

    public  int             cdStartedAt         { get; private set; } 
    public  CooldownStates  cooldownState       { get; private set; }
    public  int             currentCooldown     { get; private set; }

    public SkillInfo(Character character, Skill skill)
    {
        this.character = character;
        this.skill = skill;
        equipped = true;
        activatable = true;
    }

    public void             SetSkill        (Skill skill)                       
    {
        this.skill = skill;
    }
    public override void    SetActive       ()                                  
    {
        base.SetActive();
        UpdateCD();
    }
    public override void    SetDeactivated  ()                                  
    {
        base.SetDeactivated();
        cdStartedAt = character.timesPlayed;

        if (skill.hasPassive)
        {
            BattleManager.i.battleLog.LogBattleEffect($"{skill.name} deactivated for {character.name}.");
        }
        
        UpdateCD();
    }
    public void             UpdateCD        ()                                  
    {
        CooldownStates newState = CooldownStates.Usable;

        if (wasActivated)
        {
            int difference = character.timesPlayed - cdStartedAt;
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
    public void             CheckActivatable()                                  
    {
        activatable = ActivatableInfo.CheckActivatable(skill);
    }
}
