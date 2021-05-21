using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BaseObjectInfo
{
    public string name;
    public int id;
    [TextArea(10,30)]
    public string description;

    public BaseObjectInfo()
    {
        name = "ExampleName";
        id = -1;
    }
    public BaseObjectInfo(string name, int id)
    {
        this.name = name;
        this.id = id;
    }

    public BaseObjectInfo(string name, int id, string description)
    {
        this.name = name;
        this.id = id;
        this.description = description;
    }

    #region CustomEditor
#if UNITY_EDITOR
    [CustomEditor(typeof(BaseObjectInfo))]
    public class BaseObjectInfoCustomEditor : Editor
    {
        static BaseObjectInfo testClass;

        public override void OnInspectorGUI()
        {
            BaseObjectInfo info = testClass;

            if (info == null)
            {
                info = new BaseObjectInfo("ExampleName", 0, "ExampleDescription");
            }

            PaintBaseObjectInfo(info);

            testClass = info;
        }

        public static void PaintBaseObjectInfo(BaseObjectInfo info)
        {
            GUIStyle style = GUI.skin.textArea;
            style.wordWrap = true;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", EditorStyles.boldLabel);
            info.name = EditorGUILayout.TextField(info.name);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ID", EditorStyles.boldLabel);
            info.id = EditorGUILayout.IntField(info.id);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Description",EditorStyles.boldLabel);
            info.description = EditorGUILayout.TextArea(info.description, style);
        }
    }

#endif
    #endregion
}
