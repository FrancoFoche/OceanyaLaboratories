using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Template for any skill that is created
/// </summary>
[CreateAssetMenu(fileName = "NewSkill", menuName = "Skill")]
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
            skillInfo.SetActive();

            if (activatableType == ActivatableType.Active && firstActivation && hasPassive)
            {
                BattleManager.battleLog.LogBattleEffect($"The passive of {name} was activated for {caster.name}.");

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
            BattleManager.battleLog.LogBattleEffect($"But {caster.name} did not meet the requirements to activate the skill!");
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
                BattleManager.battleLog.LogBattleEffect("Target wasn't targettable, smh");
            }

            BattleManager.battleLog.LogBattleEffect(ReplaceActivationText(activationText));

            if (wasDefending && doesDamage)
            {
                BattleManager.battleLog.LogBattleEffect($"But {target[i].name} was defending! Meaning they actually just took {Mathf.Floor(tempDmg / 2)} DMG.");
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

    #region CustomEditor
#if UNITY_EDITOR
    [CustomEditor(typeof(Skill))]
    public class SkillCustomEditor : Editor
    {
        public static Dictionary<Behaviors, bool> behaviorShow;
        public static bool editorShowBehaviors;

        private Skill scriptableObj;
        private void OnEnable()
        {
            scriptableObj = target as Skill;
        }
        private void OnDisable()
        {
            #region Rename
            string newName = $"{scriptableObj.ID}-{scriptableObj.name}";
            target.name = newName;
            string path = AssetDatabase.GetAssetPath(target.GetInstanceID());
            AssetDatabase.RenameAsset(path, newName);
            #endregion
        }
        public override void OnInspectorGUI()
        {
            EditorUtility.SetDirty(scriptableObj);

            ActivatablesCustomEditor.PaintBaseObjectInfo(scriptableObj);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            #region RPGClass
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("RPG Class", EditorStyles.boldLabel);
            scriptableObj.skillClass = BaseSkillClass.BaseSkillClassCustomEditor.PaintSkillClassObjectSlot(scriptableObj.skillClass);
            EditorGUILayout.EndHorizontal();
            #endregion

            ActivatablesCustomEditor.PaintActivatableType(scriptableObj);

            ActivatablesCustomEditor.PaintTargets(scriptableObj);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            ActivatablesCustomEditor.PaintActivationText(scriptableObj);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            ActivatablesCustomEditor.PaintBehaviors(scriptableObj);
        }

        public static Skill PaintSkillObjectSlot(Skill skill)
        {
            skill = (Skill)EditorGUILayout.ObjectField(skill, typeof(Skill), false);

            return skill;
        }

        public static List<Skill> PaintSkillObjectList(List<Skill> skillList)
        {
            int size = Mathf.Max(0, EditorGUILayout.IntField("Skill Count", skillList.Count));

            while (size > skillList.Count)
            {
                skillList.Add(null);
            }

            while (size < skillList.Count)
            {
                skillList.RemoveAt(skillList.Count - 1);
            }

            EditorGUI.indentLevel++;
            for (int i = 0; i < skillList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Skill " + (i + 1), GUILayout.MaxWidth(100));

                Skill skill = skillList[i];
                skill = PaintSkillObjectSlot(skill);
                skillList[i] = skill;
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;

            return skillList;
        }
    }
#endif
    #endregion
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
            BattleManager.battleLog.LogBattleEffect($"{skill.name} deactivated for {character.name}.");
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

    #region CustomEditor
#if UNITY_EDITOR

    [CustomEditor(typeof(SkillInfo))]
    public class SkillInfoEditor : Editor
    {
        public static SkillInfo PaintSkillInfo(SkillInfo targetSkillinfo)
        {
            SkillInfo skillInfo = targetSkillinfo;

            EditorGUILayout.BeginHorizontal();
            skillInfo.skill = Skill.SkillCustomEditor.PaintSkillObjectSlot(skillInfo.skill);
            if (skillInfo.skill != null)
            {
                skillInfo.showInfo = EditorGUILayout.Foldout(skillInfo.showInfo, "ExtraInfo", true);
            }
            EditorGUILayout.EndHorizontal();

            if (skillInfo.showInfo && skillInfo.skill != null)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField("Editor Modifyable", EditorStyles.boldLabel);
                skillInfo.equipped = EditorGUILayout.Toggle("Equipped", skillInfo.equipped);
                skillInfo.activatable = EditorGUILayout.Toggle("Activatable", skillInfo.activatable);
                skillInfo.currentCooldown = EditorGUILayout.IntField("Current CD", skillInfo.currentCooldown);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Editor NonModifyable", EditorStyles.boldLabel);
                EditorGUILayout.Toggle("Currently Active", skillInfo.currentlyActive);
                EditorGUILayout.Toggle("Was Activated", skillInfo.wasActivated);
                EditorGUILayout.IntField("Times Activated", skillInfo.timesActivated);
                EditorGUILayout.IntField("Activated At", skillInfo.activatedAt);
                EditorGUILayout.IntField("CD Started At", skillInfo.cdStartedAt);
                EditorGUILayout.EnumPopup("CD State", skillInfo.cooldownState);
                
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel++;
            EditorGUILayout.Space();

            EditorGUI.indentLevel--;

            return skillInfo;
        }

        public static List<SkillInfo> PaintSkillInfoList(Character character, List<SkillInfo> skillList)
        {
            int size = Mathf.Max(0, EditorGUILayout.IntField("Skill Count", skillList.Count));

            while (size > skillList.Count)
            {
                skillList.Add(null);
            }

            while (size < skillList.Count)
            {
                skillList.RemoveAt(skillList.Count - 1);
            }

            EditorGUI.indentLevel++;
            for (int i = 0; i < skillList.Count; i++)
            {
                SkillInfo skillInfo = skillList[i];

                if(skillInfo == null)
                {
                    skillInfo = CreateInstance<SkillInfo>();
                    skillInfo.character = character;
                }

                skillInfo = PaintSkillInfo(skillInfo);
                skillList[i] = skillInfo;
            }
            EditorGUI.indentLevel--;

            return skillList;
        }
    }

#endif
    #endregion
}
