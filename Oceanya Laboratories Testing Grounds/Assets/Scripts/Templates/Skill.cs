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
    public BaseSkillClass                           skillClass                          { get; set; }         //RPG Class it's from

    #region Constructors
    public Skill(BaseObjectInfo baseInfo, string activationText, ActivatableType skillType, TargetType targetType, int maxTargets = 1)
    {
        this.baseInfo = baseInfo;
        this.activationText = activationText;
        this.targetType = targetType;
        this.maxTargets = maxTargets;
        activatableType = skillType;
        //Default and initializer values go here
        done = false;
        cooldown = 0;
    }
    public Skill BehaviorDoesDamage(DamageType damageType, ElementType damageElement, List<SkillFormula> damageFormula)
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
    public Skill BehaviorDoesHeal(List<SkillFormula> healFormula)
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
    public Skill BehaviorModifiesStat(StatModificationTypes modificationType, Dictionary<Stats, List<SkillFormula>> statModifiers)
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
    public Skill BehaviorChangesBasicAttack(List<SkillFormula> newBaseFormula, DamageType newDamageType)
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
                    int rawDMG = SkillFormula.ReadAndSumList(damageFormula, caster.stats);

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
                    int healAmount = SkillFormula.ReadAndSumList(healFormula, caster.stats);

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
                            resultModifiers.Add(currentStat, SkillFormula.ReadAndSumList(formulaStatModifiers[currentStat], caster.stats));
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
        static Skill Target;
        public static Dictionary<Behaviors, bool> behaviorShow;
        public static bool editorShowBehaviors;
        public override void OnInspectorGUI()
        {
            Skill newTarget = (Skill)target;
            Target = newTarget;

            ActivatablesCustomEditor.PaintBaseObjectInfo(newTarget);
            #region Rename
            string newName = $"{newTarget.baseInfo.id}-{newTarget.baseInfo.name}";
            target.name = newName;
            string path = AssetDatabase.GetAssetPath(target.GetInstanceID());
            AssetDatabase.RenameAsset(path, newName);
            #endregion

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            ActivatablesCustomEditor.PaintActivatableType(newTarget);

            ActivatablesCustomEditor.PaintTargets(newTarget);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            ActivatablesCustomEditor.PaintActivationText(newTarget);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            ActivatablesCustomEditor.PaintBehaviors(newTarget);
            Target = newTarget;
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

public class SkillFormula : ScriptableObject
{
    public Stats StatToUse { get; private set; }
    public operationActions OperationModifier { get; private set; }
    public float NumberModifier { get; private set; } 

    public SkillFormula()                                                                                       
    {
        StatToUse = Stats.STR;
        OperationModifier = operationActions.Multiply;
        NumberModifier = 1f;
    }
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

    #region CustomEditor
#if UNITY_EDITOR

    [CustomEditor(typeof(SkillFormula))]
    public class SkillFormulaCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            SkillFormula editor = (SkillFormula)target;

            PaintSkillFormula(editor);
        }

        public static SkillFormula PaintSkillFormula(SkillFormula targetskillFormula)
        {
            SkillFormula skillFormula = targetskillFormula;

            skillFormula.StatToUse = (Stats)EditorGUILayout.EnumPopup("Stat", skillFormula.StatToUse);
            skillFormula.OperationModifier = (operationActions)EditorGUILayout.EnumPopup("Operation", skillFormula.OperationModifier);
            skillFormula.NumberModifier = EditorGUILayout.FloatField("Number", skillFormula.NumberModifier);

            return skillFormula;
        }

        public static List<SkillFormula> PaintSkillFormulaList(List<SkillFormula> targetList)
        {
            List<SkillFormula> list = targetList;
            int size = Mathf.Max(0, EditorGUILayout.IntField("FormulaCount", list.Count));

            while (size > list.Count)
            {
                list.Add(null);
            }

            while (size < list.Count)
            {
                list.RemoveAt(list.Count - 1);
            }

            for (int i = 0; i < list.Count; i++)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Formula " + (i + 1), EditorStyles.boldLabel);

                SkillFormula formula = list[i];

                if (formula == null)
                {
                    formula = CreateInstance<SkillFormula>();
                }

                formula = PaintSkillFormula(formula);

                list[i] = formula;
                EditorGUI.indentLevel--;
            }

            return list;
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
            BattleManager.battleLog.LogBattleEffect($"{skill.baseInfo.name} deactivated for {character.name}.");
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
