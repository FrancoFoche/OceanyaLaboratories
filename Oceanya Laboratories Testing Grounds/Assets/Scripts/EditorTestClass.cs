using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// just a testing class, nothing here makes sense so don't worry about it
/// </summary>
public class EditorTestClass
{
    public BaseObjectInfo                           baseInfo                            { get; private set; }
    public BaseSkillClass                           skillClass                          { get; set; }         //RPG Class it's from
    public ActivatableType                                skillType                           { get; private set; }
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

    #region EditorStuff
    #if UNITY_EDITOR
    [CustomEditor(typeof(EditorTestClass))]
    public class TestEditor : Editor
    {
        List<Skill.Behaviors> behaviors;
        static EditorTestClass Target;

        public override void OnInspectorGUI()
        {
            EditorTestClass test = Target;

            if (test == null)
            {
                test = new EditorTestClass();
            }

            Target = test;

            #region BaseObjectInfo
            BaseObjectInfo info = Target.baseInfo;

            if (info == null)
            {
                info = new BaseObjectInfo("ExampleName",0,"ExampleDescription");
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
                if(i != 0)
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
                behaviors = new List<Skill.Behaviors>();
            }
            ClearBehaviors();

            EditorGUILayout.LabelField("Behaviors",EditorStyles.boldLabel);
            PaintBehaviorList(behaviors);
            #endregion
        }

        public static void PaintBehaviorList(List<Skill.Behaviors> targetList)
        {
            List<Skill.Behaviors> list = targetList;
            int size = Mathf.Max(0, EditorGUILayout.IntField("Behavior Count", list.Count));

            Skill.Behaviors defaultEnum = Skill.Behaviors.None;

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

                if(list[i] == Skill.Behaviors.None)
                {
                    list[i] = Skill.Behaviors.CostsTurn;
                }

                Skill.Behaviors behavior = list[i];
                behavior = (Skill.Behaviors)EditorGUILayout.EnumPopup("Type", behavior);

                EditorGUI.indentLevel++;
                PaintBehavior(behavior);
                EditorGUI.indentLevel--;


                list[i] = behavior;
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            }
        }

        public static void PaintBehavior(Skill.Behaviors behavior)
        {
            EditorGUI.indentLevel++;
            switch (behavior)
            {
                case Skill.Behaviors.DoesDamage:
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

                case Skill.Behaviors.HasCooldown:
                    {
                        Target.cdType = (CDType)EditorGUILayout.EnumPopup("Type", Target.cdType);

                        if(Target.cdType == CDType.Turns)
                        {
                            Target.cooldown = EditorGUILayout.IntField("TurnCount", Target.cooldown);
                        }
                    }
                    break;

                case Skill.Behaviors.DoesHeal:
                    {
                        Target.doesHeal = true;

                        if (Target.healFormula == null)
                        {
                            Target.healFormula = new List<SkillFormula>();
                        }

                        SkillFormula.SkillFormulaCustomEditor.PaintSkillFormulaList(Target.healFormula);
                    }
                    break;

                case Skill.Behaviors.FlatModifiesStat:
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

                case Skill.Behaviors.FormulaModifiesStat:
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

                case Skill.Behaviors.ModifiesResource:
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

                case Skill.Behaviors.UnlocksResource:
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

                case Skill.Behaviors.Passive:
                    {
                        Target.hasPassive = true;
                        Target.passiveActivationType = (ActivationTime)EditorGUILayout.EnumPopup("Activation Type", Target.passiveActivationType);
                    }
                    break;

                case Skill.Behaviors.CostsTurn:
                    {
                        Target.costsTurn = true;
                    }
                    break;

                case Skill.Behaviors.ActivationRequirement:
                    {
                        Target.hasActivationRequirement = true;
                        if (Target.activationRequirements == null)
                        {
                            Target.activationRequirements = new List<ActivationRequirement>();
                        }

                        ActivationRequirement.ActivationRequirementEditor.PaintActivationRequirementList(Target.activationRequirements);
                    }
                    break;

                case Skill.Behaviors.LastsFor:
                    {
                        Target.lasts = true;
                        Target.lastsFor = EditorGUILayout.IntField("Lasts For", Target.lastsFor);
                    }
                    break;

                case Skill.Behaviors.ChangesBasicAttack:
                    {
                        Target.changesBasicAttack = true;
                        Target.newBasicAttackDamageType = (DamageType)EditorGUILayout.EnumPopup("DMG Type", Target.newBasicAttackDamageType);

                        if(Target.newBasicAttackFormula == null)
                        {
                            Target.newBasicAttackFormula = new List<SkillFormula>();
                        }

                        SkillFormula.SkillFormulaCustomEditor.PaintSkillFormulaList(Target.newBasicAttackFormula);
                    }
                    break;

                case Skill.Behaviors.Revives:
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
    }

    #endif
    #endregion
}
