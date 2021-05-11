using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterType
{
    PlayerCharacter,
    Enemy
}

public enum StatModificationTypes
{
    Buff,
    Debuff
}

public class Character
{
    public string                           name                            { get; protected set; }
    public int                              level                           { get; protected set; }
    public Dictionary<Stats, int>           stats                           { get; protected set; }
    public Dictionary<SkillResources, int>  skillResources                  { get; protected set; }

    public List<SkillFormula>               basicAttackFormula              { get; protected set; }
    public DamageType                       basicAttackType                 { get; protected set; }

    public Team                             team                            { get; protected set; }
    public bool                             targettable                     { get; protected set; }

    public bool                             dead                            { get; protected set; }
    public bool                             permadead                       { get; protected set; }
    public bool                             defending                       { get; private set; }


    public List<SkillInfo>                  skillList                       { get; protected set; }
    public int                              ID                              { get; protected set; }

    public int                              timesPlayed                     { get; protected set; }

    public bool                             checkedPassives                 { get; protected set; }

    public BattleUI                         curUI                           { get; set; }
    public SpriteAnimator                   curSprite                       { get; set; }

    public Dictionary<CharActions, int>     importanceOfActions             { get; protected set; }
    public Dictionary<Skill, int>           importanceOfSkills              { get; protected set; }

    protected void InitializeVariables()
    {
        name = "InitializerName";
        level = 1;
        stats = new Dictionary<Stats, int>();
        skillResources = new Dictionary<SkillResources, int>();
        basicAttackFormula = new List<SkillFormula>() { new SkillFormula(Stats.STR, operationActions.Multiply, 1) };
        basicAttackType = DamageType.Physical;

        team = Team.Ally;
        targettable = true;

        dead = false;
        permadead = false;
        defending = false;


        skillList = new List<SkillInfo>();
        ID = -1;

        timesPlayed = 0;

        checkedPassives = false;

        curUI = null;
        curSprite = null;

        importanceOfActions = new Dictionary<CharActions, int>();
        importanceOfSkills = new Dictionary<Skill, int>();
    }

    #region Character Reactions
    public void     GetsDamagedBy           (int DamageTaken)                                                               
    {
        curUI.effectAnimator.PlayEffect(EffectAnimator.Effects.Attack);
        int dmg = DamageTaken;

        if (defending)
        {
            dmg = Mathf.FloorToInt(DamageTaken / 2);
            BattleManager.battleLog.LogBattleEffect($"But {name} was defending! Meaning they actually just took {dmg} DMG.");
            SetDefending(false);
        }

        int result = stats[Stats.CURHP] - dmg;
        if (result <= 0)
        {
            stats[Stats.CURHP] = 0;
            dead = true;
            curUI.effectAnimator.PlayEffect(EffectAnimator.Effects.Death);
        }

        if (!dead)
        {
            stats[Stats.CURHP] = result;
        }
    }
    public void     GetsHealedBy            (int HealAmount)                                                                
    {
        if (!dead)
        {
            curUI.effectAnimator.PlayEffect(EffectAnimator.Effects.Heal);

            int result = stats[Stats.CURHP] + HealAmount;

            if (result > stats[Stats.MAXHP])
            {
                stats[Stats.CURHP] = stats[Stats.MAXHP];
            }
            else
            {
                stats[Stats.CURHP] = result;
            }
        }
    }
    public void     UnlockResources         (List<SkillResources> resourcesUnlocked)                                        
    {
        for (int i = 0; i < resourcesUnlocked.Count; i++)
        {
            if (!skillResources.ContainsKey(resourcesUnlocked[i]))
            {
                skillResources.Add(resourcesUnlocked[i], 0);
            }
            
        }
    }
    public void     ModifyResource          (Dictionary<SkillResources, int> resources)                                     
    {
        for (int i = 0; i < RuleManager.SkillResourceHelper.Length; i++)
        {
            SkillResources currentResource = RuleManager.SkillResourceHelper[i];

            if (resources.ContainsKey(currentResource))
            {
                if (skillResources.ContainsKey(currentResource))
                {
                    skillResources[currentResource] += resources[currentResource];
                }
                else
                {
                    Debug.Log($"{name} did not have {currentResource.ToString()} in their unlocked resources (Ignore this for now.)");
                }
            }
        }
    }
    public void     ModifyStat              (StatModificationTypes modificationType, Dictionary<Stats, int> modifiedStats)  
    {
        for (int i = 0; i < RuleManager.StatHelper.Length; i++)
        {
            Stats currentStat = RuleManager.StatHelper[i];

            if(modifiedStats.ContainsKey(currentStat))
            {
                if(modificationType == StatModificationTypes.Buff)
                {
                    stats[currentStat] += modifiedStats[currentStat];
                    curUI.effectAnimator.PlayEffect(EffectAnimator.Effects.Buff);
                }
                else if (modificationType == StatModificationTypes.Debuff)
                {
                    stats[currentStat] -= modifiedStats[currentStat];
                    curUI.effectAnimator.PlayEffect(EffectAnimator.Effects.Debuff);
                }
            }
        }
    }
    public void     ChangeBaseAttack        (List<SkillFormula> newBaseFormula, DamageType newDamageType)                   
    {
        basicAttackFormula = newBaseFormula;
        basicAttackType = newDamageType;
    }
    public void     Revive                  ()                                                                              
    {
        if (dead)
        {
            dead = false;
            stats[Stats.CURHP] = stats[Stats.MAXHP];
            curUI.effectAnimator.PlayEffect(EffectAnimator.Effects.Revive);
        }
        else
        {
            BattleManager.battleLog.LogBattleEffect($"But {name} was not dead...");
        }
    }
    #endregion

    #region Useful Methods
    public int              CalculateDefenses(int damageRaw, DamageType damageType)
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
    public void             UpdateCDs()
    {
        for (int i = 0; i < skillList.Count; i++)
        {
            skillList[i].UpdateCD();
        }
    }
    public void             CheckPassives()
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
    public void             ActivatePassiveEffects(ActivationTime activationType)
    {
        Character character = this;

        List<string> skillnames = new List<string>();
        for (int i = 0; i < character.skillList.Count; i++)
        {
            skillnames.Add(character.skillList[i].skill.baseInfo.name);
        }


        for (int i = 0; i < character.skillList.Count; i++)
        {
            SkillInfo curSkillInfo = character.skillList[i];
            Skill curSkill = curSkillInfo.skill;
            string name = curSkill.baseInfo.name;

            if (curSkill.passiveActivationType == activationType && curSkill.hasPassive)
            {
                if(curSkill.skillType == SkillType.Active && curSkillInfo.currentlyActive || curSkill.skillType == SkillType.Passive)
                {
                    curSkill.Activate(character);

                    if (curSkill.lasts)
                    {
                        curSkillInfo.timesActivated++;

                        if (curSkillInfo.timesActivated == curSkill.lastsFor)
                        {
                            curSkillInfo.SetDeactivated();
                        }
                    }
                }
            }
        }
    }
    public List<SkillInfo>  ConvertSkillsToSkillInfo(List<Skill> skills)
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
    public SkillInfo        ConvertSkillToSkillInfo(Skill skill)
    {
        return new SkillInfo(this, skill);
    }

    public SkillInfo        GetSkillFromSkillList(Skill skill)
    {
        for (int i = 0; i < skillList.Count; i++)
        {
            if(skillList[i].skill.skillClass.baseInfo.id == skill.skillClass.baseInfo.id && skillList[i].skill.baseInfo.id == skill.baseInfo.id)
            {
                return skillList[i];
            }
        }

        Debug.LogError($"{name} did not have the skill {skill.baseInfo.name}");
        return new SkillInfo(this, skill);
    }
    #endregion

    #region Setters
    public void SetTeam                 (Team team)                                 
    {
        this.team = team;
    }
    public void SetPlayed               ()                                          
    {
        timesPlayed += !dead ? 1 : 0;
    }
    public void SetDefending            (bool mode)                                 
    {
        if (mode)
        {
            BattleManager.battleLog.LogBattleEffect($"{name} defends!");
            defending = true;
        }
        else
        {
            BattleManager.battleLog.LogBattleEffect($"{name} stops defending.");
            defending = false;
        }
    }
    public void SetCheckedPassives      (bool state)                                
    {
        checkedPassives = state;
    }
    public void SetDead                 (bool state)                                
    {
        dead = state;
    }
    /// <summary>
    /// Set the probability of an action being chosen when it is controlled by AI
    /// </summary>
    public void SetImportanceOfActions  (Dictionary<CharActions, int> importance)   
    {
        importanceOfActions = importance;
    }
    /// <summary>
    /// Set the probability of a skill being chosen when it is controlled by AI
    /// </summary>
    public void SetImportanceOfSkills   (Dictionary<Skill, int> importance)         
    {
        importanceOfSkills = importance;
    }
    #endregion


    #region AI stuff
    bool choseSkillAlready = false;
    /// <summary>
    /// Chooses and acts its whole turn on its own
    /// </summary>
    public void AITurn()
    {
        Debug.Log("AI Turn Start.");

        BattleManager.instance.CheckTotalTeamKill();

        if (!(BattleManager.instance.enemyTeamDeath || BattleManager.instance.allyTeamDeath))
        {
            //Update the importance of your actions (for now its just a default of attack = 1; skill = 3; defense = 1;)
            DynamicSetActionImportance();

            #region Choose an action
            int baseNumberOfActions = RuleManager.CharActionsHelper.Length;

            //The importance list will simply hold a certain amount of the same action, according to its importance. 
            //Meaning the more of the same action there are, the more probable it is that the action gets chosen.
            List<CharActions> importanceList = new List<CharActions>();

            for (int i = 0; i < baseNumberOfActions; i++)
            {
                CharActions currentAction = RuleManager.CharActionsHelper[i];

                if (!(currentAction == CharActions.Skill && choseSkillAlready))
                {
                    if (importanceOfActions.ContainsKey(currentAction))
                    {
                        List<CharActions> currentImportance = new List<CharActions>();

                        for (int j = 0; j < importanceOfActions[currentAction]; j++)
                        {
                            currentImportance.Add(currentAction);
                        }

                        importanceList.AddRange(currentImportance);
                    }
                }
            }

            int randomActionIndex = Random.Range(0, importanceList.Count);
            Debug.Log($"{name}'s random action index: {randomActionIndex}.");
            CharActions actionChosen = importanceList[randomActionIndex];
            Debug.Log($"{name}'s random action: {actionChosen}.");
            #endregion

            if (actionChosen == CharActions.Skill)
            {
                #region CheckIfYouCanChooseSkills
                List<Skill> cleanList = new List<Skill>();
                bool canChooseSkills = false;
                for (int i = 0; i < skillList.Count; i++)
                {
                    SkillInfo curskillInfo = skillList[i];
                    Skill curSkill = curskillInfo.skill;
                    curskillInfo.CheckActivatable();

                    if (curskillInfo.activatable && !curskillInfo.currentlyActive)
                    {
                        cleanList.Add(curSkill);
                    }
                }

                if (cleanList.Count != 0)
                {
                    canChooseSkills = true;
                }
                #endregion

                if (canChooseSkills)
                {
                    DynamicSetSkillImportance();

                    #region Choose a Skill
                        //The importance list will simply hold a certain amount of the same skill, according to its importance. 
                        //Meaning the more of the same skill there are, the more probable it is that the action gets chosen.
                        List<Skill> skillImportanceList = new List<Skill>();

                        for (int i = 0; i < cleanList.Count; i++)
                        {
                            Skill currentSkill = cleanList[i];

                            if (importanceOfSkills.ContainsKey(currentSkill))
                            {
                                List<Skill> currentImportance = new List<Skill>();

                                for (int j = 0; j < importanceOfSkills[currentSkill]; j++)
                                {
                                    currentImportance.Add(currentSkill);
                                }

                                skillImportanceList.AddRange(currentImportance);
                            }
                            else
                            {
                                //So the default importance is always 1.
                                skillImportanceList.Add(currentSkill);
                            }
                        }

                        Skill skillChosen = PickRandomSkillFromList(skillImportanceList);
                    #endregion

                    UICharacterActions.instance.ButtonAction(actionChosen);
                    UICharacterActions.instance.SetSkillToActivate(skillChosen);
                }
                else
                {
                    BattleManager.battleLog.LogBattleEffect($"{name} could not choose any skills to activate, redoing turn.");
                    choseSkillAlready = true;
                    AITurn();
                    return;
                }
            }
            else
            {
                UICharacterActions.instance.ButtonAction(actionChosen);
            }

            

            if (TeamOrderManager.turnState == TurnState.WaitingForTarget && BattleManager.caster == this)
            {
                #region Choose Targets
                int maxTargets = UICharacterActions.instance.maxTargets;
                List<Character> targets = new List<Character>();

                for (int i = 0; i < maxTargets; i++)
                {
                    Character targetChosen;

                    if (team == Team.Ally)
                    {
                        targetChosen = PickRandomAliveTargetFromList(TeamOrderManager.enemySide);
                    }
                    else
                    {
                        targetChosen = PickRandomAliveTargetFromList(TeamOrderManager.allySide);
                    }

                    targets.Add(targetChosen);
                }
                #endregion

                BattleManager.instance.SetTargets(targets);
            }

            choseSkillAlready = false;
        }
    }

    public Character PickRandomAliveTargetFromList(List<Character> listToChooseFrom)
    {
        int randomTargetIndex = Random.Range(0, TeamOrderManager.allySide.Count);
        Debug.Log($"{name}'s random target index chosen: {randomTargetIndex}.");

        Character targetChosen = TeamOrderManager.allySide[randomTargetIndex];
        Debug.Log($"{name}'s random target chosen: {targetChosen.name}.");

        if (targetChosen.dead)
        {
            Debug.Log($"Character was dead. Rechoosing.");
            return PickRandomAliveTargetFromList(listToChooseFrom);
        }
        else
        {
            return targetChosen;
        }
    }
    public Skill     PickRandomSkillFromList(List<Skill> listToChooseFrom)
    {
        int randomSkillIndex = Random.Range(0, listToChooseFrom.Count);
        Debug.Log($"{name}'s random skill index chosen: {randomSkillIndex}.");


        Skill skillChosen = listToChooseFrom[randomSkillIndex];
        Debug.Log($"{name}'s random skill: {skillChosen.baseInfo.name}.");

        return skillChosen;
    }
    /// <summary>
    /// Here is where i plan to set the AI's importances according to their health and other conditions, but for now it is simply a default importance.
    /// </summary>
    void DynamicSetActionImportance()
    {
        Dictionary<CharActions, int> normalImportance = new Dictionary<CharActions, int>()
        {
            { CharActions.Attack, 1 },
            { CharActions.Defend, 1 },
            { CharActions.Skill , 3 },
        };

        Dictionary<CharActions, int> damagedImportance = new Dictionary<CharActions, int>()
        {
            { CharActions.Attack, 1 },
            { CharActions.Defend, 4 },
            { CharActions.Skill , 2 },
        };

        importanceOfActions = normalImportance;
    }

    void DynamicSetSkillImportance()
    {
        Dictionary<Skill, int> normalImportance = new Dictionary<Skill, int>();

        for (int i = 0; i < skillList.Count; i++)
        {
            normalImportance.Add(skillList[i].skill, 1);
        }

        importanceOfSkills = normalImportance;
    }
    #endregion
}