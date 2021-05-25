using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class Activatables : ScriptableObject
{
    public BaseObjectInfo                           baseInfo                            { get; protected set; }
    public ActivatableType                          activatableType                     { get; protected set; }

    public List<Behaviors>                          behaviors                           { get; protected set; }

    public string                                   activationText                      { get; protected set; }

    public bool                                     hasActivationRequirement            { get; protected set; }
    public List<ActivationRequirement>              activationRequirements              { get; protected set; }

    public bool                                     hasPassive                          { get; protected set; }
    public ActivationTime                           passiveActivationType               { get; protected set; }

    public bool                                     lasts                               { get; protected set; }
    public int                                      lastsFor                            { get; protected set; }

    public TargetType                               targetType                          { get; protected set; } //If the skill targets anyone, what is its target type?
    public int                                      maxTargets                          { get; protected set; }

    public CDType                                   cdType                              { get; protected set; }
    public int                                      cooldown                            { get; protected set; }

    public bool                                     doesDamage                          { get; protected set; } //If the skill does damage
    public DamageType                               damageType                          { get; protected set; } //What type of damage does it do
    public ElementType                              damageElement                       { get; protected set; } //What elemental type is the damage skill
    public List<SkillFormula>                       damageFormula                       { get; protected set; } //list of formulas to sum to get the damage number

    public bool                                     doesHeal                            { get; protected set; } //if the skill heals
    public List<SkillFormula>                       healFormula                         { get; protected set; } //list of formulas to sum to get the heal number

    public bool                                     flatModifiesStat                    { get; protected set; } //does the skill buff any stat by a flat number
    public Dictionary<Stats, int>                   flatStatModifiers                   { get; protected set; }
    public bool                                     formulaModifiesStat                 { get; protected set; } //does the skill buff any stat by a formula
    public Dictionary<Stats, List<SkillFormula>>    formulaStatModifiers                { get; protected set; }
    public StatModificationTypes                    modificationType                    { get; protected set; }

    public bool                                     modifiesResource                    { get; protected set; } //does the skill modify a resource? (Mana, Bloodstacks, HP, etc.)
    public Dictionary<SkillResources, int>          resourceModifiers                   { get; protected set; } //what does it modify and by how much

    public bool                                     unlocksResource                     { get; protected set; } //does it unlock a resource
    public List<SkillResources>                     unlockedResources                   { get; protected set; } //what resources does it unlock

    public bool                                     changesBasicAttack                  { get; protected set; }
    public List<SkillFormula>                       newBasicAttackFormula               { get; protected set; }
    public DamageType                               newBasicAttackDamageType            { get; protected set; }

    public bool                                     revives                             { get; protected set; }

    public bool                                     costsTurn                           { get; protected set; } //does the skill end your turn

    public bool                                     appliesStatusEffects                { get; protected set; } //does the skill apply a status effect?

    public bool                                     doesSummon                          { get; protected set; } //does the skill summon anything

    public bool                                     doesShield                          { get; protected set; } //does the skill shield anything

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

    public abstract void    Activate                    (Character caster);

    public abstract void    Action                      (Character caster, List<Character> target);

    public string           ReplaceActivationText       (Dictionary<ReplaceStringVariables, string> replaceWith)                                                            
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
    [CustomEditor(typeof(Activatables))]
    public class ActivatablesCustomEditor : Editor
    {
        static Activatables Target;
        public static Dictionary<Behaviors, bool> behaviorShow;
        public static bool editorShowBehaviors;
        public override void OnInspectorGUI()
        {
            Activatables newTarget = (Activatables)target;
            Target = newTarget;


            PaintBaseObjectInfo(newTarget);
            #region Rename
            string newName = $"{newTarget.baseInfo.id}-{newTarget.baseInfo.name}";
            target.name = newName;
            string path = AssetDatabase.GetAssetPath(target.GetInstanceID());
            AssetDatabase.RenameAsset(path, newName);
            #endregion

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            PaintActivatableType(newTarget);

            PaintTargets(newTarget);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            PaintActivationText(newTarget);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            PaintBehaviors(newTarget);

            Target = newTarget;
        }

        public static void PaintBaseObjectInfo(Activatables activatable)
        {
            EditorGUILayout.LabelField("Base Info", EditorStyles.boldLabel);

            BaseObjectInfo newInfo = activatable.baseInfo;
            
            if (newInfo == null)
            {
                Debug.Log("skill baseinfo was null");
                newInfo = CreateInstance<BaseObjectInfo>();
                EditorUtility.SetDirty(newInfo);
                UnityEditor.AssetDatabase.SaveAssets();
            }

            EditorGUI.indentLevel++;
            newInfo = BaseObjectInfo.BaseObjectInfoCustomEditor.PaintBaseObjectInfo(newInfo);
            EditorGUI.indentLevel--;

            activatable.baseInfo = newInfo;
        }
        public static void PaintActivatableType(Activatables activatable)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Activatable Type", EditorStyles.boldLabel);
            activatable.activatableType = (ActivatableType)EditorGUILayout.EnumPopup(activatable.activatableType);
            EditorGUILayout.EndHorizontal();
        }
        public static void PaintTargets(Activatables activatable)
        {
            Activatables newTarget = activatable;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Target Type", EditorStyles.boldLabel);
            newTarget.targetType = (TargetType)EditorGUILayout.EnumPopup(newTarget.targetType);
            EditorGUILayout.EndHorizontal();

            switch (newTarget.targetType)
            {
                case TargetType.Multiple:
                    newTarget.maxTargets = EditorGUILayout.IntField("Max Targets", newTarget.maxTargets);
                    break;
            }
            activatable = newTarget;
        }
        public static void PaintActivationText(Activatables activatable)
        {
            Activatables newTarget = activatable;
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
            newTarget.activationText = EditorGUILayout.TextField(newTarget.activationText, style);

            activatable = newTarget;
        }
        public static void PaintBehaviors(Activatables activatable)
        {
            Activatables newTarget = activatable;
            Target = newTarget;
            ClearBehaviors(newTarget);
            editorShowBehaviors = EditorGUILayout.Foldout(editorShowBehaviors, "Behaviors", EditorStyles.boldFont);
            if (editorShowBehaviors)
            {
                EditorGUI.indentLevel++;
                newTarget.behaviors = PaintBehaviorList(newTarget.behaviors);
                EditorGUI.indentLevel--;
            }
            activatable = newTarget;
        }

        public static int behaviorOptionIndex;
        public static int behaviorPickedIndex;
        public static List<Behaviors> PaintBehaviorList(List<Behaviors> targetList)
        {
            if (targetList == null)
            {
                targetList = new List<Behaviors>();
            }

            if (behaviorShow == null)
            {
                behaviorShow = new Dictionary<Behaviors, bool>();
            }

            List<Behaviors> list = targetList;

            List<Behaviors> options = new List<Behaviors>();
            List<Behaviors> alreadyPicked = new List<Behaviors>();

            #region Check for which behaviors are available to add and which are already picked
            for (int i = 0; i < RuleManager.SkillBehaviorHelper.Length; i++)
            {
                Behaviors curBehavior = RuleManager.SkillBehaviorHelper[i];

                if (curBehavior != Behaviors.None)
                {
                    if (!list.Contains(curBehavior))
                    {
                        options.Add(curBehavior);
                    }
                    else
                    {
                        if (!alreadyPicked.Contains(curBehavior))
                        {
                            alreadyPicked.Add(curBehavior);
                        }
                    }
                }

                if (!behaviorShow.ContainsKey(curBehavior))
                {
                    behaviorShow.Add(curBehavior, false);
                }
            }
            #endregion

            string[] stringOptions = new string[options.Count];
            string[] stringAlreadyPicked = new string[alreadyPicked.Count];

            #region Transform the lists into string arrays so you can use them as popups
            for (int i = 0; i < stringOptions.Length; i++)
            {
                stringOptions[i] = options[i].ToString();
            }
            for (int i = 0; i < stringAlreadyPicked.Length; i++)
            {
                stringAlreadyPicked[i] = alreadyPicked[i].ToString();
            }
            #endregion

            #region If you don't already have every skill behavior, make an Add Behavior section
            if (alreadyPicked.Count != RuleManager.SkillBehaviorHelper.Length - 1)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Add", EditorStyles.boldLabel, GUILayout.MaxWidth(55f));
                behaviorOptionIndex = EditorGUILayout.Popup(behaviorOptionIndex, stringOptions);
                Behaviors curBehavior = options[behaviorOptionIndex];
                if (GUILayout.Button("Add " + curBehavior.ToString()))
                {
                    list.Add(curBehavior);
                    alreadyPicked.Add(curBehavior);
                }
                EditorGUILayout.EndHorizontal();
            }
            #endregion

            #region If you have some skill behaviors, make a remove resource section
            if (list.Count != 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Remove", EditorStyles.boldLabel, GUILayout.MaxWidth(55f));
                behaviorPickedIndex = EditorGUILayout.Popup(behaviorPickedIndex, stringAlreadyPicked);
                if (GUILayout.Button("Remove " + alreadyPicked[behaviorPickedIndex].ToString()))
                {
                    list.Remove(alreadyPicked[behaviorPickedIndex]);
                }
                EditorGUILayout.EndHorizontal();
            }
            #endregion

            #region For every skill resource you have, add a line to set the resource
            for (int i = 0; i < list.Count; i++)
            {
                Behaviors behavior = list[i];
                if(behavior == Behaviors.None)
                {
                    behavior = Behaviors.CostsTurn;
                }

                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Behavior " + (i + 1) + ": " + behavior.ToString(), EditorStyles.boldLabel);

                behaviorShow[behavior] = EditorGUILayout.Foldout(behaviorShow[behavior], "Info", true);
                EditorGUILayout.EndHorizontal();

                if (behaviorShow[behavior])
                {
                    EditorGUI.indentLevel++;
                    PaintBehavior(behavior);
                    EditorGUI.indentLevel--;
                }

                list[i] = behavior;
                EditorGUI.indentLevel--;

            }
            #endregion
            return list;
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

                        Target.damageFormula = SkillFormula.SkillFormulaCustomEditor.PaintSkillFormulaList(Target.damageFormula);
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

                        Target.healFormula = SkillFormula.SkillFormulaCustomEditor.PaintSkillFormulaList(Target.healFormula);
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

                            Target.formulaStatModifiers[curStat] = SkillFormula.SkillFormulaCustomEditor.PaintSkillFormulaList(Target.formulaStatModifiers[curStat]);
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

                        Target.activationRequirements = ActivationRequirement.ActivationRequirementEditor.PaintActivationRequirementList(Target.activationRequirements);
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

                        Target.newBasicAttackFormula = SkillFormula.SkillFormulaCustomEditor.PaintSkillFormulaList(Target.newBasicAttackFormula);
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
        public static void ClearBehaviors(Activatables Target)
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

public class ActivatableInfo : ScriptableObject
{
    public Character    character       { get; protected set; }
    public bool         activatable     { get; protected set; }
    public bool         equipped        { get; protected set; }
    public bool         wasActivated    { get; protected set; }   //If the skill was activated at SOME point.
    public bool         currentlyActive { get; protected set; }   //If the skill is currently active
    public int          activatedAt     { get; protected set; }   //when the skill was activated
    public int          timesActivated  { get; set; }           //how many times the skill was activated
    public bool         showInfo        { get; protected set; }

    public void Equip()
    {
        equipped = true;
    }
    public void Unequip()
    {
        equipped = false;
    }
    public virtual void SetActive()
    {
        currentlyActive = true;
        wasActivated = true;
        activatedAt = character.timesPlayed;
    }
    public virtual void SetDeactivated()
    {
        timesActivated = 0;
        currentlyActive = false;
    }
    public static bool CheckActivatable(Activatables activatable)
    {
        bool activatableBool = true;

        if (activatable.hasActivationRequirement)
        {
            for (int i = 0; i < activatable.activationRequirements.Count; i++)
            {
                if (!activatable.activationRequirements[i].CheckRequirement())
                {
                    activatableBool = false;
                }
            }

        }

        return activatableBool;
    }
}
public class ActivationRequirement : ScriptableObject
{
    public RequirementType type { get; set; }

    public SkillResources resource { get; private set; }
    public Stats stat { get; private set; }
    //add a status one here whenever it's done
    public int skillclassID { get; private set; }
    public int skillID { get; private set; }
    public Skill skill { get; private set; }
    public ComparerType comparer { get; private set; }
    public int number { get; private set; }

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

    public enum RequirementType
    {
        Stat,
        Resource,
        Status,
        SkillIsActive
    }
    public enum ComparerType
    {
        MoreThan,
        LessThan,
        Equal
    }

    public bool CheckRequirement()
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
                if (skill == null)
                {
                    skill = GameAssetsManager.instance.GetSkill(skillclassID, skillID);
                }
                return caster.GetSkillFromSkillList(skill).currentlyActive;

            default:
                Debug.LogError("Invalid Requirement type, returning true");
                return true;
        }
    }
    public bool CheckRequirement(int number)
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
            editor = (ActivationRequirement)target;

            editor = PaintActivationRequirement(editor);
        }

        public static ActivationRequirement PaintActivationRequirement(ActivationRequirement targetActRequirement)
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

            return actRequirement;
        }

        public static List<ActivationRequirement> PaintActivationRequirementList(List<ActivationRequirement> targetList)
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
                    actRequirement = CreateInstance<ActivationRequirement>();
                }

                PaintActivationRequirement(actRequirement);

                list[i] = actRequirement;
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            }

            return list;
        }
    }

#endif
    #endregion
}