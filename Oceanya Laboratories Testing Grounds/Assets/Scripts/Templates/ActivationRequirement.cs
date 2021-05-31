using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "NewActivationRequirement", menuName = "Rules/Activation Requirement")]
public class ActivationRequirement : ScriptableObject
{
    [SerializeField] private RequirementType _type;

    [SerializeField] private SkillResources _resource;
    [SerializeField] private Stats _stat;
    //add a status one here whenever it's done
    [SerializeField] private int _skillclassID;
    [SerializeField] private int _skillID;
    [SerializeField] private Skill _skill;
    [SerializeField] private ComparerType _comparer;
    [SerializeField] private int _number;

    #region Getter/Setters
    public RequirementType      type            { get { return _type; }             private set { _type = value; } }

    public SkillResources       resource        { get { return _resource; }         private set { _resource = value; } }
    public Stats                stat            { get { return _stat; }             private set { _stat = value; } }
    
    public int                  skillclassID    { get { return _skillclassID; }     private set { _skillclassID = value; } }
    public int                  skillID         { get { return _skillID; }          private set { _skillID = value; } }
    public Skill                skill           { get { return _skill; }            private set { _skill = value; } }
    public ComparerType         comparer        { get { return _comparer; }         private set { _comparer = value; } }
    public int                  number          { get { return _number; }           private set { _number = value; } }
    #endregion

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
        ActivationRequirement Target;
        private void OnEnable()
        {
            Target = target as ActivationRequirement;
        }

        private void OnDisable()
        {
            #region Rename
            string newName = GetActivationRequirementFileName(Target);
            string path = AssetDatabase.GetAssetPath(target.GetInstanceID());
            AssetDatabase.RenameAsset(path, newName);
            #endregion
        }

        public override void OnInspectorGUI()
        {
            EditorUtility.SetDirty(Target);
            ActivationRequirement editor = Target;

            editor = PaintActivationRequirement(editor);

            Target = editor;
        }

        public static string GetActivationRequirementFileName(ActivationRequirement targetActRequirement)
        {
            string newName = "";

            switch (targetActRequirement.type)
            {
                case RequirementType.Stat:
                    {
                        newName = $"If {targetActRequirement.stat} is ";
                        switch (targetActRequirement.comparer)
                        {
                            case ComparerType.MoreThan:
                                newName += "more than ";
                                break;
                            case ComparerType.LessThan:
                                newName += "less than ";
                                break;
                            case ComparerType.Equal:
                                newName += "equal to ";
                                break;
                        }
                        newName += targetActRequirement.number.ToString();
                    }
                    break;

                case RequirementType.Resource:
                    {
                        newName = $"If {targetActRequirement.resource} is ";
                        switch (targetActRequirement.comparer)
                        {
                            case ComparerType.MoreThan:
                                newName += "more than ";
                                break;
                            case ComparerType.LessThan:
                                newName += "less than ";
                                break;
                            case ComparerType.Equal:
                                newName += "equal to ";
                                break;
                        }
                        newName += targetActRequirement.number.ToString();
                    }
                    break;

                case RequirementType.Status:
                    newName = "Has StatusEffect not yet implemented";
                    break;

                case RequirementType.SkillIsActive:
                    Skill skill = GameAssetsManager.instance.GetSkill(targetActRequirement.skillclassID, targetActRequirement.skillID);
                    newName = "If " + skill.name + " is Active";
                    break;

                default:
                    break;
            }

            return newName;
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

        public static ActivationRequirement PaintActivationRequirementObjectSlot(ActivationRequirement actRequirement)  
        {
            actRequirement = (ActivationRequirement)EditorGUILayout.ObjectField(actRequirement, typeof(ActivationRequirement), false);

            return actRequirement;
        }

        public static List<ActivationRequirement> PaintActivationRequirementObjectList(List<ActivationRequirement> list)
        {
            int size = Mathf.Max(0, EditorGUILayout.IntField("Count", list.Count));

            while (size > list.Count)
            {
                list.Add(null);
            }

            while (size < list.Count)
            {
                list.RemoveAt(list.Count - 1);
            }

            EditorGUI.indentLevel++;
            for (int i = 0; i < list.Count; i++)
            {
                ActivationRequirement newSlot = list[i];
                newSlot = PaintActivationRequirementObjectSlot(newSlot);
                list[i] = newSlot;
            }
            EditorGUI.indentLevel--;

            return list;
        }
    }

#endif
    #endregion
}
