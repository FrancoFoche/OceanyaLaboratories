using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "NewPlayerCharacter", menuName = "Characters/PlayerCharacter")]
public class PlayerCharacter : Character
{
    public BaseSkillClass rpgClass;
    public PlayerCharacter(int ID, string name, int level, BaseSkillClass rpgClass, Dictionary<Stats, int> stats, List<Skill> skillList, List<Item> inventory)
    {
        InitializeVariables();

        this.ID = ID;
        this.level = level;
        this.name = name;
        this.rpgClass = rpgClass;
        this.stats = stats;
        this.inventory = ConvertItemsToItemInfo(inventory);
        this.skillList = ConvertSkillsToSkillInfo(skillList);
    }

    #region CustomEditor
#if UNITY_EDITOR
    [CustomEditor(typeof(PlayerCharacter))]
    public class PlayerCharacterCustomEditor : Editor
    {
        PlayerCharacter Target;

        public override void OnInspectorGUI()
        {
            Target = (PlayerCharacter)target;

            Target = PaintPlayerCharacter(Target);
        }

        public PlayerCharacter PaintPlayerCharacter(PlayerCharacter target)
        {
            PlayerCharacter character = target;

            CharacterCustomEditor.PaintBasicCharacterInfo(character);

            #region RpgClass
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Class", EditorStyles.boldLabel);
            character.rpgClass = (BaseSkillClass)EditorGUILayout.ObjectField(character.rpgClass, typeof(BaseSkillClass), false);
            EditorGUILayout.EndHorizontal();
            #endregion

            CharacterCustomEditor.PaintBasicAttack(character);

            RuleManager.BuildHelpers();

            CharacterCustomEditor.PaintStats(character);

            CharacterCustomEditor.PaintSkillResources(character);

            CharacterCustomEditor.PaintInventory(character);

            CharacterCustomEditor.PaintCharacterSkillList(character);

            return character;
        }
    }
#endif
    #endregion

}
