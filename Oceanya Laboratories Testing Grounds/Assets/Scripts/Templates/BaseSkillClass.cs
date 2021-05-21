﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "NewClass", menuName = "RPGClass")]
public class BaseSkillClass: ScriptableObject
{
    public BaseObjectInfo baseInfo;
    public List<Skill> skillList;
    public Dictionary<string, int> statBoosts = new Dictionary<string, int>();

    public BaseSkillClass()
    {
        baseInfo = new BaseObjectInfo();
        skillList = new List<Skill>();
    }
    public BaseSkillClass(BaseObjectInfo baseInfo, List<Skill> skillList)
    {
        this.baseInfo = baseInfo;
        this.skillList = skillList;
    }

    public BaseSkillClass(BaseObjectInfo baseInfo, List<Skill> skillList, Dictionary<string, int> statBoosts)
    {
        this.baseInfo = baseInfo;
        this.skillList = skillList;
        this.statBoosts = statBoosts;
    }

    #region CustomEditor
#if UNITY_EDITOR
    [CustomEditor(typeof(BaseSkillClass))]
    public class BaseObjectInfoCustomEditor : Editor
    {
        static BaseSkillClass rpgClass;

        public override void OnInspectorGUI()
        {
            rpgClass = (BaseSkillClass)target;

            #region BaseInfo
            EditorGUILayout.LabelField("Base Info", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            BaseObjectInfo info = rpgClass.baseInfo;
            if(info == null)
            {
                info = new BaseObjectInfo();
            }

            BaseObjectInfo.BaseObjectInfoCustomEditor.PaintBaseObjectInfo(info);
            rpgClass.baseInfo = info;
            EditorGUI.indentLevel--;
            #endregion

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            #region SkillList
            EditorGUILayout.LabelField("Skill List", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;


            List<Skill> skills = rpgClass.skillList;
            if(skills == null)
            {
                skills = new List<Skill>();
            }
            skills = Skill.SkillCustomEditor.PaintSkillObjectList(skills);
            rpgClass.skillList = skills;
            EditorGUI.indentLevel--;
            #endregion
        }
    }

#endif
    #endregion
}
