using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "NewClass", menuName = "RPGClass")]
public class BaseSkillClass: ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private int _ID;
    [SerializeField] private string _description;

    public List<Skill> skillList;
    public Dictionary<string, int> statBoosts = new Dictionary<string, int>();

    public new string   name                { get { return _name; }         protected set { _name = value; } }
    public int          ID                  { get { return _ID; }           protected set { _ID = value; } }
    public string       description         { get { return _description; }  protected set { _description = value; } }

    public BaseSkillClass(BaseObjectInfo baseInfo, List<Skill> skillList)
    {
        name = baseInfo.name;
        ID = baseInfo.id;
        description = baseInfo.description;
        this.skillList = skillList;
    }

    #region CustomEditor
#if UNITY_EDITOR
    [CustomEditor(typeof(BaseSkillClass))]
    public class BaseSkillClassCustomEditor : Editor
    {
        static BaseSkillClass Target;
        private void OnEnable()
        {
            Target = target as BaseSkillClass;
        }
        private void OnDisable()
        {
            #region Rename
            string newName = $"{Target.ID}-{Target.name}";
            target.name = newName;
            string path = AssetDatabase.GetAssetPath(target.GetInstanceID());
            AssetDatabase.RenameAsset(path, newName);
            #endregion
        }
        public override void OnInspectorGUI()
        {
            EditorUtility.SetDirty(Target);

            #region BaseInfo
            EditorGUILayout.LabelField("Base Info", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            GUIStyle style = GUI.skin.textArea;
            style.wordWrap = true;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", EditorStyles.boldLabel);
            Target.name = EditorGUILayout.TextField(Target.name);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ID", EditorStyles.boldLabel);
            Target.ID = EditorGUILayout.IntField(Target.ID);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Description", EditorStyles.boldLabel);
            Target.description = EditorGUILayout.TextArea(Target.description, style);
            EditorGUI.indentLevel--;
            #endregion

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            #region SkillList
            EditorGUILayout.LabelField("Skill List", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;


            List<Skill> skills = Target.skillList;
            if(skills == null)
            {
                skills = new List<Skill>();
            }
            skills = Skill.SkillCustomEditor.PaintSkillObjectList(skills);
            Target.skillList = skills;
            EditorGUI.indentLevel--;
            #endregion
        }

        public static BaseSkillClass PaintSkillClassObjectSlot(BaseSkillClass rpgClass)
        {
            rpgClass = (BaseSkillClass)EditorGUILayout.ObjectField(rpgClass, typeof(BaseSkillClass), false);

            return rpgClass;
        }
    }

#endif
    #endregion
}
