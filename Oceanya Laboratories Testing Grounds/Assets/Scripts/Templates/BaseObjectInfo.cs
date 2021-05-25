using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BaseObjectInfo : ScriptableObject
{
    public new string name;
    public int id;
    public string description;

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

            info = PaintBaseObjectInfo(info);

            testClass = info;
        }

        public static BaseObjectInfo PaintBaseObjectInfo(BaseObjectInfo info)
        {
            BaseObjectInfo newInfo = info;
            GUIStyle style = GUI.skin.textArea;
            style.wordWrap = true;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", EditorStyles.boldLabel);
            newInfo.name = EditorGUILayout.TextField(newInfo.name);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ID", EditorStyles.boldLabel);
            newInfo.id = EditorGUILayout.IntField(newInfo.id);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Description",EditorStyles.boldLabel);
            newInfo.description = EditorGUILayout.TextArea(newInfo.description, style);

            if(newInfo.name != info.name || newInfo.id != info.id || newInfo.description != info.description)
            {
                Debug.Log($"Skill Info change. Old: Name = {info.name}; ID = {info.id}; Description = {info.description}; NEW: {newInfo.name}, {newInfo.id}, {newInfo.description}");
            }
            info = newInfo;
            return info;
        }
    }

#endif
    #endregion
}
