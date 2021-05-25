using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Characters/Enemy")]
public class Enemy : Character
{
    public Sprite image;

    public Enemy(int ID, string name, Sprite image, int level, Dictionary<Stats, int> stats, List<Skill> skillList, List<Item> items)
    {
        InitializeVariables();

        this.ID = ID;
        this.level = level;
        this.name = name;
        this.stats = stats;
        this.image = image;
        this.inventory = ConvertItemsToItemInfo(items);

        this.skillList = ConvertSkillsToSkillInfo(skillList);
    }

    #region CustomEditor
#if UNITY_EDITOR
    [CustomEditor(typeof(Enemy))]
    public class EnemyCustomEditor : Editor
    {
        Enemy Target;

        public override void OnInspectorGUI()
        {
            Target = (Enemy)target;

            Target = PaintEnemy(Target);
        }

        public Enemy PaintEnemy(Enemy target)
        {
            Enemy character = target;

            CharacterCustomEditor.PaintBasicCharacterInfo(character);

            #region Sprite
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Base Sprite", EditorStyles.boldLabel);
            character.image = (Sprite)EditorGUILayout.ObjectField(character.image, typeof(Sprite), false);
            EditorGUILayout.EndHorizontal();
            #endregion

            CharacterCustomEditor.PaintBasicAttack(character);

            RuleManager.BuildHelpers();

            CharacterCustomEditor.PaintStats(character);

            CharacterCustomEditor.PaintSkillResources(character);

            CharacterCustomEditor.PaintCharacterSkillList(character);
            
            return character;
        }
    }
#endif
    #endregion
}
