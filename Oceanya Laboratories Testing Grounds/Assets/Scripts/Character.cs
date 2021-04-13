﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    public string                           name;
    public int                              level;
    public Dictionary<Stats, int>           stats               =   new Dictionary<Stats, int>();
    public Dictionary<SkillResources, int>  skillResources      = 
                                                                    new Dictionary<SkillResources, int>() 
                                                                    {
                                                                        { SkillResources.Bloodstacks, 0 },
                                                                        { SkillResources.Mana, 0 },
                                                                        { SkillResources.NatureEnergy, 0 },
                                                                        { SkillResources.other, 0 },
                                                                        { SkillResources.Puppets, 0 }
                                                                    };

    public List<SkillFormula>               basicAttackFormula  =   new List<SkillFormula>(){new SkillFormula(Stats.STR, SkillFormula.operationActions.Multiply, 1)};
    public DamageType                       basicAttackType     =   DamageType.Physical;

    public Team                             team;
    public bool                             targettable         =   true; //if the target is targettable currently

    public bool                             dead;
    public bool                             permadead;

    public Dictionary<SkillResources, bool> unlockedResources =     new Dictionary<SkillResources, bool>()
                                                                    {
                                                                        { SkillResources.Bloodstacks, false },
                                                                        { SkillResources.Mana, false },
                                                                        { SkillResources.NatureEnergy, false },
                                                                        { SkillResources.other, false },
                                                                        { SkillResources.Puppets, false }
                                                                    };

    #region Character Reactions
    public void     GetsDamagedBy       (int DamageTaken)
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
    public void     GetsHealedBy        (int HealAmount)
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
    public int      CalculateDefenses   (int damageRaw, DamageType damageType, Character target)
    {
        int targetMR = target.stats[Stats.MR];
        int targetPR = target.stats[Stats.PR];

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
    public void     UnlockResources     (List<SkillResources> resourcesUnlocked)
    {
        for (int i = 0; i < resourcesUnlocked.Count; i++)
        {
            unlockedResources[resourcesUnlocked[i]] = true;
        }
    }
    public void     ModifyResource      (Dictionary<SkillResources, int> resources)
    {
        for (int i = 0; i < RuleManager.SkillResourceHelper.Count; i++)
        {
            SkillResources currentResource = RuleManager.SkillResourceHelper[i];

            if (resources.ContainsKey(currentResource))
            {
                skillResources[currentResource] += resources[currentResource];
            }
        }
    }
    public void     ModifyStat          (Dictionary<Stats, int> modifiedStats)
    {
        for (int i = 0; i < RuleManager.StatHelper.Count; i++)
        {
            Stats currentStat = RuleManager.StatHelper[i];

            if(modifiedStats.ContainsKey(currentStat))
            {
                stats[currentStat] += modifiedStats[currentStat];
            }
        }
    }
    #endregion

    #region Character Actions
    public int      Attack              (PlayerCharacter target)
    {
        int basicAttackRaw = SkillFormula.ReadAndSumList(basicAttackFormula, stats);
        int resultDMG = CalculateDefenses(basicAttackRaw, basicAttackType, target);
        target.GetsDamagedBy(resultDMG);
        return resultDMG;
    }
    #endregion
}
