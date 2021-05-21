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
public class Skill: ScriptableObject
{
    /// <summary>
    /// This is just for testing purposes, any skill that has this boolean as "true" means that it currently works as intended. You can get all skills that are done through the skill database function "GetAllDoneSkills"
    /// </summary>
    public bool                                     done                                { get; private set; }

    public BaseObjectInfo                           baseInfo                            { get; private set; }
    public BaseSkillClass                           skillClass                          { get; set; }         //RPG Class it's from
    public SkillType                                skillType                           { get; private set; }

    public string                                   activationText                      { get; private set; }

    public bool                                     hasActivationRequirement            { get; private set; }
    public List<ActivationRequirement>              activationRequirements              { get; private set; }

    public bool                                     hasPassive                          { get; private set; }
    public ActivationTime                           passiveActivationType               { get; private set; }

    public bool                                     lasts                               { get; private set; }
    public int                                      lastsFor                            { get; private set; }

    public TargetType                               targetType                          { get; private set; } //If the skill targets anyone, what is its target type?
    public int                                      maxTargets                          { get; private set; }

    public CDType                                   cdType                              { get; private set; }
    public int                                      cooldown                            { get; private set; }

    public bool                                     doesDamage                          { get; private set; } //If the skill does damage
    public DamageType                               damageType                          { get; private set; } //What type of damage does it do
    public ElementType                              damageElement                       { get; private set; } //What elemental type is the damage skill
    public List<SkillFormula>                       damageFormula                       { get; private set; } //list of formulas to sum to get the damage number

    public bool                                     doesHeal                            { get; private set; } //if the skill heals
    public List<SkillFormula>                       healFormula                         { get; private set; } //list of formulas to sum to get the heal number

    public bool                                     flatModifiesStat                    { get; private set; } //does the skill buff any stat by a flat number
    public Dictionary<Stats, int>                   flatStatModifiers                   { get; private set; }
    public bool                                     formulaModifiesStat                 { get; private set; } //does the skill buff any stat by a formula
    public Dictionary<Stats, List<SkillFormula>>    formulaStatModifiers                { get; private set; }
    public StatModificationTypes                    modificationType                    { get; private set; }

    public bool                                     modifiesResource                    { get; private set; } //does the skill modify a resource? (Mana, Bloodstacks, HP, etc.)
    public Dictionary<SkillResources, int>          resourceModifiers                   { get; private set; } //what does it modify and by how much

    public bool                                     unlocksResource                     { get; private set; } //does it unlock a resource
    public List<SkillResources>                     unlockedResources                   { get; private set; } //what resources does it unlock

    public bool                                     changesBasicAttack                  { get; private set; }
    public List<SkillFormula>                       newBasicAttackFormula               { get; private set; }
    public DamageType                               newBasicAttackDamageType            { get; private set; }

    public bool                                     revives                             { get; private set; }

    public bool                                     costsTurn                           { get; private set; } //does the skill end your turn

    public bool                                     appliesStatusEffects                { get; private set; } //does the skill apply a status effect?

    public bool                                     doesSummon                          { get; private set; } //does the skill summon anything

    public bool                                     doesShield                          { get; private set; } //does the skill shield anything

    public enum Behaviors
    {
        None,
        DoesDamage,
        HasCooldown,
        DoesHeal,
        FlatModifiesStat,
        FormulaModifiesStat,
        ModifiesResource,
        UnlocksResource,
        Passive,
        CostsTurn,
        ActivationRequirement,
        LastsFor,
        ChangesBasicAttack,
        Revives
    }

    #region Constructors
    public Skill                                (BaseObjectInfo baseInfo, string activationText, SkillType skillType, TargetType targetType, int maxTargets = 1)    
    {
        this.baseInfo = baseInfo;
        this.activationText = activationText;
        this.targetType = targetType;
        this.maxTargets = maxTargets;
        this.skillType = skillType;


        //Default and initializer values go here
        done = false;
        cooldown = 0;
    }
    public Skill BehaviorDoesDamage             (DamageType damageType, ElementType damageElement, List<SkillFormula> damageFormula)                                
    {
        doesDamage = true;
        this.damageType = damageType;
        this.damageElement = damageElement;
        this.damageFormula = damageFormula;
        return this;
    }
    public Skill BehaviorHasCooldown            (CDType cdType)                                                                                                     
    {
        this.cdType = cdType;
        return this;
    }
    public Skill BehaviorHasCooldown            (CDType cdType, int cooldown)                                                                                       
    {
        this.cdType = cdType;
        this.cooldown = cooldown;
        return this;
    }
    public Skill BehaviorDoesHeal               (List<SkillFormula> healFormula)                                                                                    
    {
        doesHeal = true;
        this.healFormula = healFormula;
        return this;
    }
    public Skill BehaviorModifiesStat           (StatModificationTypes modificationType, Dictionary<Stats, int> statModifiers)                                                                              
    {
        flatModifiesStat = true;
        flatStatModifiers = statModifiers;
        this.modificationType = modificationType;
        return this;
    }
    public Skill BehaviorModifiesStat           (StatModificationTypes modificationType, Dictionary<Stats, List<SkillFormula>> statModifiers)                                                                     
    {
        formulaModifiesStat = true;
        formulaStatModifiers = statModifiers;
        this.modificationType = modificationType;

        return this;
    }
    public Skill BehaviorModifiesResource       (Dictionary<SkillResources, int> resourceModifiers)                                                                 
    {
        modifiesResource = true;
        this.resourceModifiers = resourceModifiers;
        return this;
    }
    public Skill BehaviorUnlocksResource        (List<SkillResources> unlockedResources)                                                                            
    {
        unlocksResource = true;
        this.unlockedResources = unlockedResources;
        return this;
    }
    public Skill BehaviorPassive                (ActivationTime activationType)                                                                                     
    {
        hasPassive = true;
        this.passiveActivationType = activationType;
        return this;
    }
    public Skill BehaviorCostsTurn              ()                                                                                                                  
    {
        costsTurn = true;
        return this;
    }
    public Skill BehaviorActivationRequirement  (List<ActivationRequirement> requirements)                                                                          
    {
        hasActivationRequirement = true;
        activationRequirements = requirements;
        return this;
    }
    public Skill BehaviorLastsFor               (int maxActivationTimes)                                                                                            
    {
        lasts = true;
        lastsFor = maxActivationTimes;

        return this;
    }
    public Skill BehaviorChangesBasicAttack     (List<SkillFormula> newBaseFormula, DamageType newDamageType)                                                       
    {
        this.newBasicAttackFormula = newBaseFormula;
        this.newBasicAttackDamageType = newDamageType;
        changesBasicAttack = true;
        return this;
    }
    public Skill BehaviorRevives                ()                                                                                                                  
    {
        revives = true;
        return this;
    }

    public Skill BehaviorAppliesStatusEffects   ()                                                                                                                  
    {
        appliesStatusEffects = true;
        return this;
    }
    public Skill BehaviorDoesSummon             ()                                                                                                                  
    {
        doesSummon = true;
        return this;
    }
    public Skill BehaviorDoesShield             ()                                                                                                                  
    {
        doesShield = true;
        return this;
    }
    
    /// <summary>
    /// just marks the skill as done, this is just for development purposes
    /// </summary>
    /// <returns></returns>
    public Skill IsDone                         ()                                                                                                                  
    {
        done = true;
        return this;
    }
    #endregion

    public void     Activate                    (Character caster)                                                                                                  
    {
        SkillInfo skillInfo = caster.GetSkillFromSkillList(this);

        skillInfo.CheckActivatable();

        if (skillInfo.activatable)
        {
            bool firstActivation = !skillInfo.currentlyActive;
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
        }
        else
        {
            BattleManager.battleLog.LogBattleEffect($"But {caster.name} did not meet the requirements to activate the skill!");
        }
    }

    public void     SkillAction                 (Character caster, List<Character> target)                                                                          
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

                    target[i].ModifyStat(modificationType ,resultModifiers);
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


        if (skillType != SkillType.Passive && !hasPassive)
        {
            caster.GetSkillFromSkillList(this).SetDeactivated();
        }

        if (costsTurn)
        {
            if (!(skillType == SkillType.Active && hasPassive && caster.GetSkillFromSkillList(this).currentlyActive))
            {
                TeamOrderManager.EndTurn();
            }
        }
    }

    public string   ReplaceActivationText       (Dictionary<ReplaceStringVariables, string> replaceWith)                                                            
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

    #region CustomEditor
    #if UNITY_EDITOR
    [CustomEditor(typeof(Skill))]
    public class SkillCustomEditor : Editor
    {
        List<Behaviors> behaviors;
        static Skill Target;

        public override void OnInspectorGUI()
        {
            Target = (Skill)target;

            #region BaseObjectInfo
            BaseObjectInfo info = Target.baseInfo;

            if (info == null)
            {
                info = new BaseObjectInfo("ExampleName", 0, "ExampleDescription");
            }

            EditorGUILayout.LabelField("Base Info", EditorStyles.boldLabel);
            BaseObjectInfo.BaseObjectInfoCustomEditor.PaintBaseObjectInfo(info);
            Target.baseInfo = info;
            #endregion

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            #region Targets
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Target Type", EditorStyles.boldLabel);
            Target.targetType = (TargetType)EditorGUILayout.EnumPopup(Target.targetType);
            EditorGUILayout.EndHorizontal();

            switch (Target.targetType)
            {
                case TargetType.Multiple:
                    Target.maxTargets = EditorGUILayout.IntField("Max Targets", Target.maxTargets);
                    break;
            }
            #endregion

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            #region Activation Texts
            GUIStyle style = GUI.skin.textArea;
            style.wordWrap = true;

            string tooltipText = "The text that is sent to the chat when the skill is activated. Before sending, it replaces certain values with the results of the skill. The variables are: \n";
            string variables = "";
            RuleManager.BuildHelpers();
            for (int i = 0; i < RuleManager.ReplaceableStringsHelper.Length; i++)
            {
                if (i != 0)
                {
                    variables += ",\n";
                }

                variables += RuleManager.ReplaceableStringsHelper[i].ToString();
            }

            EditorGUILayout.LabelField(new GUIContent("Activation Text", tooltipText + variables), EditorStyles.boldLabel);
            Target.activationText = EditorGUILayout.TextField(Target.activationText, style);
            #endregion

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            #region Behaviors
            if (behaviors == null)
            {
                behaviors = new List<Behaviors>();
            }
            ClearBehaviors();

            EditorGUILayout.LabelField("Behaviors", EditorStyles.boldLabel);
            PaintBehaviorList(behaviors);
            #endregion
        }

        public static void PaintBehaviorList(List<Behaviors> targetList)
        {
            List<Behaviors> list = targetList;
            int size = Mathf.Max(0, EditorGUILayout.IntField("Behavior Count", list.Count));

            Behaviors defaultEnum = Behaviors.None;

            while (size > list.Count)
            {
                list.Add(defaultEnum);
            }

            while (size < list.Count)
            {
                list.RemoveAt(list.Count - 1);
            }

            for (int i = 0; i < list.Count; i++)
            {
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Behavior " + (i + 1), EditorStyles.boldLabel);
                EditorGUILayout.Space();

                if (list[i] == Behaviors.None)
                {
                    list[i] = Behaviors.CostsTurn;
                }

                Behaviors behavior = list[i];
                behavior = (Behaviors)EditorGUILayout.EnumPopup("Type", behavior);

                EditorGUI.indentLevel++;
                PaintBehavior(behavior);
                EditorGUI.indentLevel--;


                list[i] = behavior;
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            }
        }

        public static void PaintBehavior(Behaviors behavior)
        {
            EditorGUI.indentLevel++;
            switch (behavior)
            {
                case Behaviors.DoesDamage:
                    {
                        Target.doesDamage = true;

                        Target.damageType = (DamageType)EditorGUILayout.EnumPopup("Type", Target.damageType);
                        Target.damageElement = (ElementType)EditorGUILayout.EnumPopup("Element", Target.damageElement);


                        if (Target.damageFormula == null)
                        {
                            Target.damageFormula = new List<SkillFormula>();
                        }

                        SkillFormula.SkillFormulaCustomEditor.PaintSkillFormulaList(Target.damageFormula);
                    }
                    break;

                case Behaviors.HasCooldown:
                    {
                        Target.cdType = (CDType)EditorGUILayout.EnumPopup("Type", Target.cdType);

                        if (Target.cdType == CDType.Turns)
                        {
                            Target.cooldown = EditorGUILayout.IntField("TurnCount", Target.cooldown);
                        }
                    }
                    break;

                case Behaviors.DoesHeal:
                    {
                        Target.doesHeal = true;

                        if (Target.healFormula == null)
                        {
                            Target.healFormula = new List<SkillFormula>();
                        }

                        SkillFormula.SkillFormulaCustomEditor.PaintSkillFormulaList(Target.healFormula);
                    }
                    break;

                case Behaviors.FlatModifiesStat:
                    {
                        Target.flatModifiesStat = true;

                        if (Target.flatStatModifiers == null)
                        {
                            Target.flatStatModifiers = new Dictionary<Stats, int>();
                        }

                        RuleManager.BuildHelpers();
                        for (int i = 0; i < RuleManager.StatHelper.Length; i++)
                        {
                            Stats curStat = RuleManager.StatHelper[i];

                            if (Target.flatStatModifiers.ContainsKey(curStat))
                            {
                                Target.flatStatModifiers[curStat] = EditorGUILayout.IntField(curStat.ToString(), Target.flatStatModifiers[curStat]);
                            }
                            else
                            {
                                Target.flatStatModifiers.Add(curStat, 0);
                                Target.flatStatModifiers[curStat] = EditorGUILayout.IntField(curStat.ToString(), Target.flatStatModifiers[curStat]);
                            }
                        }
                    }
                    break;

                case Behaviors.FormulaModifiesStat:
                    {
                        Target.formulaModifiesStat = true;

                        if (Target.formulaStatModifiers == null)
                        {
                            Target.formulaStatModifiers = new Dictionary<Stats, List<SkillFormula>>();
                        }

                        RuleManager.BuildHelpers();
                        for (int i = 0; i < RuleManager.StatHelper.Length; i++)
                        {
                            Stats curStat = RuleManager.StatHelper[i];
                            EditorGUILayout.LabelField(curStat.ToString(), EditorStyles.boldLabel);

                            if (!Target.formulaStatModifiers.ContainsKey(curStat))
                            {
                                Target.formulaStatModifiers.Add(curStat, new List<SkillFormula>());
                            }

                            SkillFormula.SkillFormulaCustomEditor.PaintSkillFormulaList(Target.formulaStatModifiers[curStat]);
                            EditorGUILayout.Space();
                        }
                    }
                    break;

                case Behaviors.ModifiesResource:
                    {
                        Target.modifiesResource = true;

                        if (Target.resourceModifiers == null)
                        {
                            Target.resourceModifiers = new Dictionary<SkillResources, int>();
                        }

                        RuleManager.BuildHelpers();
                        for (int i = 0; i < RuleManager.SkillResourceHelper.Length; i++)
                        {
                            SkillResources curResource = RuleManager.SkillResourceHelper[i];

                            if (Target.resourceModifiers.ContainsKey(curResource))
                            {
                                Target.resourceModifiers[curResource] = EditorGUILayout.IntField(curResource.ToString(), Target.resourceModifiers[curResource]);
                            }
                            else
                            {
                                Target.resourceModifiers.Add(curResource, 0);
                                Target.resourceModifiers[curResource] = EditorGUILayout.IntField(curResource.ToString(), Target.resourceModifiers[curResource]);
                            }
                        }
                    }
                    break;

                case Behaviors.UnlocksResource:
                    {
                        Target.unlocksResource = true;

                        List<SkillResources> list = Target.unlockedResources;

                        if (list == null)
                        {
                            list = new List<SkillResources>();
                        }


                        int size = Mathf.Max(0, EditorGUILayout.IntField("ResourceCount", list.Count));

                        while (size > list.Count)
                        {
                            list.Add(SkillResources.none);
                        }

                        while (size < list.Count)
                        {
                            list.RemoveAt(list.Count - 1);
                        }

                        for (int i = 0; i < list.Count; i++)
                        {
                            SkillResources actRequirement = list[i];

                            if (actRequirement == SkillResources.none)
                            {
                                actRequirement = SkillResources.Mana;
                            }

                            actRequirement = (SkillResources)EditorGUILayout.EnumPopup("Resource", actRequirement);

                            list[i] = actRequirement;
                        }
                    }
                    break;

                case Behaviors.Passive:
                    {
                        Target.hasPassive = true;
                        Target.passiveActivationType = (ActivationTime)EditorGUILayout.EnumPopup("Activation Type", Target.passiveActivationType);
                    }
                    break;

                case Behaviors.CostsTurn:
                    {
                        Target.costsTurn = true;
                    }
                    break;

                case Behaviors.ActivationRequirement:
                    {
                        Target.hasActivationRequirement = true;
                        if (Target.activationRequirements == null)
                        {
                            Target.activationRequirements = new List<ActivationRequirement>();
                        }

                        ActivationRequirement.ActivationRequirementEditor.PaintActivationRequirementList(Target.activationRequirements);
                    }
                    break;

                case Behaviors.LastsFor:
                    {
                        Target.lasts = true;
                        Target.lastsFor = EditorGUILayout.IntField("Lasts For", Target.lastsFor);
                    }
                    break;

                case Behaviors.ChangesBasicAttack:
                    {
                        Target.changesBasicAttack = true;
                        Target.newBasicAttackDamageType = (DamageType)EditorGUILayout.EnumPopup("DMG Type", Target.newBasicAttackDamageType);

                        if (Target.newBasicAttackFormula == null)
                        {
                            Target.newBasicAttackFormula = new List<SkillFormula>();
                        }

                        SkillFormula.SkillFormulaCustomEditor.PaintSkillFormulaList(Target.newBasicAttackFormula);
                    }
                    break;

                case Behaviors.Revives:
                    {
                        Target.revives = true;
                    }
                    break;

                default:
                    break;
            }
            EditorGUI.indentLevel--;
        }

        public static void ClearBehaviors()
        {
            Target.hasActivationRequirement = false;

            Target.hasPassive = false;

            Target.lasts = false;

            Target.doesDamage = false;
            Target.doesHeal = false;

            Target.flatModifiesStat = false;
            Target.formulaModifiesStat = false;

            Target.modifiesResource = false;

            Target.unlocksResource = false;

            Target.changesBasicAttack = false;

            Target.revives = false;

            Target.costsTurn = false;

            Target.appliesStatusEffects = false;

            Target.doesSummon = false;

            Target.doesShield = false;
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

public class SkillFormula
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
        SkillFormula editor;

        public override void OnInspectorGUI()
        {
            if (editor == null)
            {
                editor = new SkillFormula(Stats.STR,operationActions.Multiply,1);
            }

            PaintSkillFormula(editor);
        }

        public static void PaintSkillFormula(SkillFormula targetskillFormula)
        {
            SkillFormula skillFormula = targetskillFormula;

            skillFormula.StatToUse = (Stats)EditorGUILayout.EnumPopup("Stat", skillFormula.StatToUse);
            skillFormula.OperationModifier = (operationActions)EditorGUILayout.EnumPopup("Operation", skillFormula.OperationModifier);
            skillFormula.NumberModifier = EditorGUILayout.FloatField("Number", skillFormula.NumberModifier);
        }

        public static void PaintSkillFormulaList(List<SkillFormula> targetList)
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
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Formula " + (i + 1), EditorStyles.boldLabel);

                SkillFormula formula = list[i];

                if (formula == null)
                {
                    formula = new SkillFormula(Stats.STR, operationActions.Multiply, 1);
                }

                PaintSkillFormula(formula);

                list[i] = formula;
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            }
        }
    }

#endif
    #endregion
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
    public  bool            wasActivated        { get; private set; }   //If the skill was activated at SOME point.
    public  bool            currentlyActive     { get; private set; }   //If the skill is currently active
    public  int             activatedAt         { get; private set; }   //when the skill was activated
    public  int             cdStartedAt         { get; private set; } 
    public  int             timesActivated      { get; set; }           //how many times the skill was activated
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
        timesActivated = 0;
        currentlyActive = false;
        cdStartedAt = character.timesPlayed;

        if (skill.hasPassive)
        {
            BattleManager.battleLog.LogBattleEffect($"{skill.baseInfo.name} deactivated for {character.name}.");
        }
        
        UpdateCD();
    }
    public void UpdateCD        ()                                  
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
    public  RequirementType type        { get; set; }

    public  SkillResources  resource    { get; private set; }
    public  Stats           stat        { get; private set; }
    //add a status one here whenever it's done
    public  int             skillclassID{ get; private set; }
    public  int             skillID     { get; private set; }
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
        SetSkill(skillclassID, skillID);
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

    #region Setters
    public void SetStat(Stats stat)
    {
        this.stat = stat;
    }

    public void SetComparerType(ComparerType comparer)
    {
        this.comparer = comparer;
    }

    public void SetNumber(int number)
    {
        this.number = number;
    }

    public void SetResource(SkillResources resource)
    {
        this.resource = resource;
    }

    public void SetSkill(int classID, int skillID)
    {
        skillclassID = classID;
        this.skillID = skillID;
    }
    #endregion

    #region CustomEditor
    #if UNITY_EDITOR

    [CustomEditor(typeof(ActivationRequirement))]
    public class ActivationRequirementEditor : Editor
    {
        ActivationRequirement editor;

        public override void OnInspectorGUI()
        {
            if(editor == null)
            {
                editor = new ActivationRequirement(0, 0);
            }

            PaintActivationRequirement(editor);
        }

        public static void PaintActivationRequirement(ActivationRequirement targetActRequirement)
        {
            ActivationRequirement actRequirement = targetActRequirement;

            actRequirement.type = (RequirementType)EditorGUILayout.EnumPopup("Type", actRequirement.type);

            EditorGUI.indentLevel++;

            switch (actRequirement.type)
            {
                case RequirementType.Stat:
                    actRequirement.SetStat((Stats)EditorGUILayout.EnumPopup("Stat", actRequirement.stat));
                    actRequirement.SetComparerType((ComparerType)EditorGUILayout.EnumPopup("CompareType", actRequirement.comparer));
                    actRequirement.SetNumber(EditorGUILayout.IntField("Number", actRequirement.number));
                    break;

                case RequirementType.Resource:
                    actRequirement.SetResource((SkillResources)EditorGUILayout.EnumPopup("Resource", actRequirement.resource));
                    actRequirement.SetComparerType((ActivationRequirement.ComparerType)EditorGUILayout.EnumPopup("CompareType", actRequirement.comparer));
                    actRequirement.SetNumber(EditorGUILayout.IntField("Number", actRequirement.number));
                    break;

                case RequirementType.Status:
                    EditorGUILayout.LabelField("Not yet implemented, sorry.");
                    break;

                case RequirementType.SkillIsActive:
                    actRequirement.SetSkill(EditorGUILayout.IntField("Skill Class ID", actRequirement.skillclassID), EditorGUILayout.IntField("Skill ID", actRequirement.skillID));
                    break;

                default:
                    break;
            }
            EditorGUILayout.Space();

            EditorGUI.indentLevel--;
        }

        public static void PaintActivationRequirementList(List<ActivationRequirement> targetList)
        {
            List<ActivationRequirement> list = targetList;
            int size = Mathf.Max(0, EditorGUILayout.IntField("RequirementCount", list.Count));

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
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Requirement " + (i + 1), EditorStyles.boldLabel);
                EditorGUILayout.Space();

                ActivationRequirement actRequirement = list[i];

                if (actRequirement == null)
                {
                    actRequirement = new ActivationRequirement(0, 0);
                }

                PaintActivationRequirement(actRequirement);

                list[i] = actRequirement;
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            }
        }
    }

#endif
    #endregion
}
