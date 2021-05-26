using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "NewRPGFormula", menuName = "Rules/Formula")]
public class RPGFormula : ScriptableObject
{
    public Stats StatToUse { get; private set; }
    public operationActions OperationModifier { get; private set; }
    public float NumberModifier { get; private set; }

    public RPGFormula(Stats StatToUse, operationActions OperationModifier, float NumberModifier)
    {
        this.StatToUse = StatToUse;
        this.OperationModifier = OperationModifier;
        this.NumberModifier = NumberModifier;
    }


    public static int ReadAndSumList(List<RPGFormula> formulas, Dictionary<Stats, int> stats)
    {
        int result = 0;

        for (int i = 0; i < formulas.Count; i++)
        {
            result += Read(formulas[i], stats);
        }

        return result;
    }

    public static int Read(RPGFormula skillFormula, Dictionary<Stats, int> stats)
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

    public static string FormulaToString(RPGFormula skillFormula)
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

    public static string FormulaListToString(List<RPGFormula> skillFormulas)
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

    [CustomEditor(typeof(RPGFormula))]
    public class RPGFormulaCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            RPGFormula editor = (RPGFormula)target;

            PaintSkillFormula(editor);
            #region Rename
            string newName = GetRPGFormulaFileName(editor);
            string path = AssetDatabase.GetAssetPath(target.GetInstanceID());
            AssetDatabase.RenameAsset(path, newName);
            #endregion
        }

        public static string GetRPGFormulaFileName(RPGFormula targetActRequirement)
        {
            string newName = "";

            newName = $"{targetActRequirement.StatToUse}";
            switch (targetActRequirement.OperationModifier)
            {
                case operationActions.Multiply:
                    newName += " multiplied by ";
                    break;
                case operationActions.Divide:
                    newName += " divided by ";
                    break;
                case operationActions.ToThePowerOf:
                    newName += " to the power of ";
                    break;
            }
            newName += targetActRequirement.NumberModifier.ToString();

            return newName;
        }

        public static RPGFormula PaintSkillFormula(RPGFormula targetskillFormula)
        {
            RPGFormula skillFormula = targetskillFormula;

            skillFormula.StatToUse = (Stats)EditorGUILayout.EnumPopup("Stat", skillFormula.StatToUse);
            skillFormula.OperationModifier = (operationActions)EditorGUILayout.EnumPopup("Operation", skillFormula.OperationModifier);
            skillFormula.NumberModifier = EditorGUILayout.FloatField("Number", skillFormula.NumberModifier);

            return skillFormula;
        }

        public static List<RPGFormula> PaintSkillFormulaList(List<RPGFormula> targetList)
        {
            List<RPGFormula> list = targetList;
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

                RPGFormula formula = list[i];

                if (formula == null)
                {
                    formula = CreateInstance<RPGFormula>();
                }

                formula = PaintSkillFormula(formula);

                list[i] = formula;
                EditorGUI.indentLevel--;
            }

            return list;
        }

        public static RPGFormula PaintObjectSlot(RPGFormula actRequirement)
        {
            actRequirement = (RPGFormula)EditorGUILayout.ObjectField(actRequirement, typeof(RPGFormula), false);

            return actRequirement;
        }

        public static List<RPGFormula> PaintObjectList(List<RPGFormula> list)
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
                RPGFormula newSlot = list[i];
                newSlot = PaintObjectSlot(newSlot);
                list[i] = newSlot;
            }
            EditorGUI.indentLevel--;

            return list;
        }
    }

    #endif
    #endregion
}
