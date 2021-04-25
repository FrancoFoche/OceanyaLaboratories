using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    public string                   name;
    public int                      level;
    public Dictionary<Stats, int>   stats               =   new Dictionary<Stats, int>();
    Dictionary<SkillResources, int> skillResources      = 
                                                                    new Dictionary<SkillResources, int>() 
                                                                    {
                                                                        { SkillResources.Bloodstacks, 0 },
                                                                        { SkillResources.Mana, 0 },
                                                                        { SkillResources.NatureEnergy, 0 },
                                                                        { SkillResources.other, 0 },
                                                                        { SkillResources.Puppets, 0 }
                                                                    };

    public List<SkillFormula>       basicAttackFormula  =   new List<SkillFormula>(){new SkillFormula(Stats.STR, operationActions.Multiply, 1)};
    public DamageType                      basicAttackType     =   DamageType.Physical;

    public Team                     team;
    public bool                     targettable         =   true; //if the target is targettable currently

    public bool                     dead;
    bool                            permadead;

    Dictionary<SkillResources, bool> unlockedResources =     new Dictionary<SkillResources, bool>()
                                                                    {
                                                                        { SkillResources.Bloodstacks, false },
                                                                        { SkillResources.Mana, false },
                                                                        { SkillResources.NatureEnergy, false },
                                                                        { SkillResources.other, false },
                                                                        { SkillResources.Puppets, false }
                                                                    };


    public List<SkillInfo> skillList = new List<SkillInfo>();

    public BaseSkillClass rpgClass;
    public int ID;

    public int timesPlayed;

    public bool checkedPassives = false;

    #region Character Reactions
    public void     GetsDamagedBy           (int DamageTaken)
    {
        int result = stats[Stats.CURHP] - DamageTaken;
        if (result < 0)
        {
            stats[Stats.CURHP] = 0;
            dead = true;
        }

        if (!dead)
        {
            stats[Stats.CURHP] = result;
        }
    }
    public void     GetsHealedBy            (int HealAmount)
    {
        int result = stats[Stats.CURHP] += HealAmount;

        if (!dead)
        {
            if(result > stats[Stats.MAXHP])
            {
                stats[Stats.CURHP] = stats[Stats.MAXHP];
            }
            else
            {
                stats[Stats.CURHP] = result;
            }
        }
    }
    public int      CalculateDefenses       (int damageRaw, DamageType damageType)
    {
        int targetMR = stats[Stats.MR];
        int targetPR = stats[Stats.PR];

        float defensePercentRatio = 0.25f; // Ratio of defense % per point in MR or PR. (Example: with 10 PR you get 2.5% defense against physical types.)
        float resultDefensePercent = 0; //The defense you have against whatever damage type it is.
        float defendedDamage; //Damage that was cancelled due to defense
        float resultDamage; //post defense calculation

        switch (damageType)
        {
            case DamageType.Direct:
                resultDefensePercent = 0;
                break;

            case DamageType.Magical:
                resultDefensePercent = targetMR * defensePercentRatio;
                break;

            case DamageType.Physical:
                resultDefensePercent = targetPR * defensePercentRatio;
                break;

            default:
                Debug.Log("Something went wrong i can feel it.");
                break;
        }

        defendedDamage = (resultDefensePercent * damageRaw) / 100; //Calculating the actual damage that was cancelled by using a simple rule of 3

        resultDamage = damageRaw - defendedDamage;

        return (int)Mathf.Ceil(resultDamage);
    }
    public void     UnlockResources         (List<SkillResources> resourcesUnlocked)
    {
        for (int i = 0; i < resourcesUnlocked.Count; i++)
        {
            unlockedResources[resourcesUnlocked[i]] = true;
        }
    }
    public void     ModifyResource          (Dictionary<SkillResources, int> resources)
    {
        for (int i = 0; i < RuleManager.SkillResourceHelper.Length; i++)
        {
            SkillResources currentResource = RuleManager.SkillResourceHelper[i];

            if (resources.ContainsKey(currentResource))
            {
                skillResources[currentResource] += resources[currentResource];
            }
        }
    }
    public void     ModifyStat              (Dictionary<Stats, int> modifiedStats)
    {
        for (int i = 0; i < RuleManager.StatHelper.Length; i++)
        {
            Stats currentStat = RuleManager.StatHelper[i];

            if(modifiedStats.ContainsKey(currentStat))
            {
                stats[currentStat] += modifiedStats[currentStat];
            }
        }
    }
    #endregion

    #region Useful Methods
    public void UpdateCDs()
    {
        for (int i = 0; i < skillList.Count; i++)
        {
            skillList[i].UpdateCD();
        }
    }
    public void CheckPassives()
    {
        Character character = this;
        for (int i = 0; i < character.skillList.Count; i++)
        {
            if (character.skillList[i].skill.skillType == SkillType.Passive)
            {
                character.skillList[i].SetActive();
            }
        }
    }

    /// <summary>
    /// Checks to activate a passive from a character's list IF its actuvation type matches the activation type you gave it
    /// </summary>
    /// <param name="character"></param>
    /// <param name="activationType"></param>
    public void ActivatePassiveEffects(ActivationTime activationType)
    {
        Character character = this;
        for (int i = 0; i < character.skillList.Count; i++)
        {
            SkillInfo curSkillInfo = character.skillList[i];
            Skill curSkill = curSkillInfo.skill;
            string curName = curSkill.baseInfo.name;

            if (curSkill.passiveActivationType == activationType && curSkill.hasPassive)
            {
                if(curSkill.skillType == SkillType.Active && curSkillInfo.currentlyActive || curSkill.skillType == SkillType.Passive)
                {
                    curSkill.Activate(character);
                }
            }
        }
    }
    public List<SkillInfo> ConvertSkillsToSkillInfo(List<Skill> skills)
    {
        List<SkillInfo> newList = new List<SkillInfo>();

        for (int i = 0; i < skills.Count; i++)
        {
            newList.Add(ConvertSkillToSkillInfo(skills[i]));
        }

        return newList;
    }

    /// <summary>
    /// converts a skill type to a skill info type
    /// </summary>
    public SkillInfo ConvertSkillToSkillInfo(Skill skill)
    {
        return new SkillInfo(this, skill);
    }

    public SkillInfo GetSkillFromSkillList(Skill skill)
    {
        for (int i = 0; i < skillList.Count; i++)
        {
            if(skillList[i].skill == skill)
            {
                return skillList[i];
            }
        }

        Debug.LogError($"{name} did not have the skill {skill.baseInfo.name}");
        return new SkillInfo(this, skill);
    }
    #endregion
}