using System.Collections;
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
    public Skill.DamageType                 basicAttackType     =   Skill.DamageType.Physical;

    public Team                             team;
    public bool                             targettable         =   true; //if the target is targettable currently

    public bool                             dead;
    public bool                             permadead;

    public bool                             hasNatureEnergy;
    public bool                             hasMana;
    public bool                             hasBloodstacks;
    public bool                             hasPuppets;
    public bool                             hasOtherResource;

    public enum     Team
    {
        Enemy,
        Ally,
        OutOfCombat
    }
    public enum     SkillResources
    {
        NatureEnergy,
        Mana,
        Bloodstacks,
        Puppets,
        other
    }
    public enum     Stats
    {
        CURHP,
        MAXHP,
        STR,
        INT,
        CHR,
        AGI,
        MR,
        PR,
        CON,
        HPREGEN
    }

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
    public int      CalculateDefenses   (int damageRaw, Skill.DamageType damageType, Character target)
    {
        int targetMR = target.stats[Stats.MR];
        int targetPR = target.stats[Stats.PR];

        float defensePercentRatio = 0.25f; // Ratio of defense % per point in MR or PR. (Example: with 10 PR you get 2.5% defense against physical types.)
        float resultDefensePercent = 0; //The defense you have against whatever damage type it is.
        float defendedDamage; //Damage that was cancelled due to defense
        float resultDamage; //post defense calculation

        switch (damageType)
        {
            case Skill.DamageType.Direct:
                resultDefensePercent = 0;
                break;

            case Skill.DamageType.Magical:
                resultDefensePercent = targetMR * defensePercentRatio;
                break;

            case Skill.DamageType.Physical:
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
    public string   UnlockResources     (Dictionary<SkillResources, bool> resources)
    {
        string unlockedList = "";

        if (resources[SkillResources.NatureEnergy])
        {
            hasNatureEnergy = true;
            unlockedList += " Unlocked NatureEnergy!";
        }
        
        if(resources[SkillResources.Mana])
        {
            hasMana = true;
            unlockedList += " Unlocked Mana!";
        }

        if (resources[SkillResources.Bloodstacks])
        {
            hasBloodstacks = true;
            unlockedList += " Unlocked BloodStacks!";
        }

        if (resources[SkillResources.Puppets])
        {
            hasPuppets = true;
            unlockedList += " Unlocked Puppets!";
        }

        if (resources[SkillResources.other])
        {
            hasOtherResource = true;
            unlockedList += " Unlocked... wait what did you unlock- (Other Resource)";
        }

        return unlockedList;
    }
    public string   AddToResource       (Dictionary<SkillResources, int> resources)
    {
        string resultText = "";

        if (hasNatureEnergy)
        {
            skillResources[SkillResources.NatureEnergy] += resources[SkillResources.NatureEnergy];
            resultText += $" +{resources[SkillResources.NatureEnergy]} Nature Energy!";
        }

        if (hasMana)
        {
            skillResources[SkillResources.Mana] += resources[SkillResources.Mana];
            resultText += $" +{resources[SkillResources.Mana]} Mana!";
        }

        if (hasBloodstacks)
        {
            skillResources[SkillResources.Bloodstacks] += resources[SkillResources.Bloodstacks];
            resultText += $" +{resources[SkillResources.NatureEnergy]} BloodStacks!";
        }

        if (hasPuppets)
        {
            skillResources[SkillResources.Puppets] += resources[SkillResources.Puppets];
            resultText += $" +{resources[SkillResources.NatureEnergy]} Puppets!";
        }

        if (hasOtherResource)
        {
            skillResources[SkillResources.other] += resources[SkillResources.other];
            resultText += $" +{resources[SkillResources.NatureEnergy]} Other Resource!";
        }

        return resultText;
    }


    //Character Actions
    public int      Attack              (PlayerCharacter target)
    {
        int basicAttackRaw = SkillFormula.ReadAndSumList(basicAttackFormula, stats);
        int resultDMG = CalculateDefenses(basicAttackRaw, basicAttackType, target);
        target.GetsDamagedBy(resultDMG);
        return resultDMG;
    }
}
