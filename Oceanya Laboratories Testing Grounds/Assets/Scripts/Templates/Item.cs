using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "NewItem", menuName = "Item")]
public class Item : Activatables
{
    public override void Activate(Character caster)
    {
        ItemInfo itemInfo = caster.GetItemFromInventory(this);

        itemInfo.CheckActivatable();

        if (itemInfo.activatable)
        {
            bool firstActivation = !itemInfo.currentlyActive;
            itemInfo.SetActive();

            if (activatableType == ActivatableType.Active && firstActivation && hasPassive)
            {
                BattleManager.battleLog.LogBattleEffect($"The passive of {name} was activated for {caster.name}.");

                if (costsTurn)
                {
                    TeamOrderManager.EndTurn();
                }
            }
            else
            {
                switch (targetType)
                {
                    case TargetType.Self:
                        Action(caster, new List<Character>() { caster });
                        break;

                    case TargetType.Single:
                    case TargetType.Multiple:
                        UICharacterActions.instance.maxTargets = maxTargets;
                        UICharacterActions.instance.ActionRequiresTarget(CharActions.Skill);
                        break;

                    case TargetType.AllAllies:
                        if (caster.team == Team.Ally)
                        {
                            Action(caster, TeamOrderManager.allySide);
                        }
                        else
                        {
                            Action(caster, TeamOrderManager.enemySide);
                        }
                        break;
                    case TargetType.AllEnemies:
                        if (caster.team == Team.Ally)
                        {
                            Action(caster, TeamOrderManager.enemySide);
                        }
                        else
                        {
                            Action(caster, TeamOrderManager.allySide);
                        }
                        break;
                    case TargetType.Bounce:
                        Action(caster, new List<Character>() { BattleManager.caster });
                        break;
                }
            }
        }
        else
        {
            BattleManager.battleLog.LogBattleEffect($"But {caster.name} did not meet the requirements to activate the skill!");
        }
    }

    public override void Action(Character caster, List<Character> target)
    {
        for (int i = 0; i < target.Count; i++)
        {
            Dictionary<ReplaceStringVariables, string> activationText = new Dictionary<ReplaceStringVariables, string>();

            activationText.Add(ReplaceStringVariables._caster_, caster.name);
            activationText.Add(ReplaceStringVariables._target_, target[i].name);

            int tempDmg = 0;
            bool wasDefending = false;
            if (target[i].targettable)
            {
                if (doesDamage)
                {
                    int rawDMG = RPGFormula.ReadAndSumList(damageFormula, caster.stats);

                    int finalDMG = target[i].CalculateDefenses(rawDMG, damageType);
                    tempDmg = finalDMG;
                    if (target[i].defending)
                    {
                        wasDefending = true;
                    }

                    target[i].GetsDamagedBy(finalDMG);

                    activationText.Add(ReplaceStringVariables._damage_, finalDMG.ToString());
                }
                if (doesHeal)
                {
                    int healAmount = RPGFormula.ReadAndSumList(healFormula, caster.stats);

                    target[i].GetsHealedBy(healAmount);

                    activationText.Add(ReplaceStringVariables._heal_, healAmount.ToString());
                }
                if (flatModifiesStat)
                {
                    target[i].ModifyStat(modificationType, flatStatModifiers);
                }
                if (formulaModifiesStat)
                {
                    Dictionary<Stats, int> resultModifiers = new Dictionary<Stats, int>();
                    for (int j = 0; j < RuleManager.StatHelper.Length; j++)
                    {
                        Stats currentStat = RuleManager.StatHelper[j];

                        if (formulaStatModifiers.ContainsKey(currentStat))
                        {
                            resultModifiers.Add(currentStat, RPGFormula.ReadAndSumList(formulaStatModifiers[currentStat], caster.stats));
                        }
                    }

                    target[i].ModifyStat(modificationType, resultModifiers);
                }
                if (unlocksResource)
                {
                    target[i].UnlockResources(unlockedResources);
                }
                if (modifiesResource)
                {
                    target[i].ModifyResource(resourceModifiers);
                }
                if (changesBasicAttack)
                {
                    target[i].ChangeBaseAttack(newBasicAttackFormula, newBasicAttackDamageType);
                }
                if (revives)
                {
                    target[i].Revive();
                }

                if (appliesStatusEffects)
                {

                }

                if (doesSummon)
                {

                }

                if (doesShield)
                {

                }
            }
            else
            {
                BattleManager.battleLog.LogBattleEffect("Target wasn't targettable, smh");
            }

            BattleManager.battleLog.LogBattleEffect(ReplaceActivationText(activationText));

            if (wasDefending && doesDamage)
            {
                BattleManager.battleLog.LogBattleEffect($"But {target[i].name} was defending! Meaning they actually just took {Mathf.Floor(tempDmg / 2)} DMG.");
            }

        }


        if (activatableType != ActivatableType.Passive && !hasPassive)
        {
            caster.GetItemFromInventory(this).SetDeactivated();
        }

        if (costsTurn)
        {
            if (!(activatableType == ActivatableType.Active && hasPassive && caster.GetItemFromInventory(this).currentlyActive))
            {
                TeamOrderManager.EndTurn();
            }
        }
    }

    #region CustomEditor
#if UNITY_EDITOR
    [CustomEditor(typeof(Item))]
    public class ItemCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Item item = (Item)target;

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            item = PaintItem(item);

            #region Rename
            string newName = $"{item.ID}-{item.name}";
            target.name = newName;
            string path = AssetDatabase.GetAssetPath(target.GetInstanceID());
            AssetDatabase.RenameAsset(path, newName);
            #endregion
        }

        public Item PaintItem(Item item)
        {
            ActivatablesCustomEditor.PaintBaseObjectInfo(item);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            ActivatablesCustomEditor.PaintActivatableType(item);

            ActivatablesCustomEditor.PaintTargets(item);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            ActivatablesCustomEditor.PaintActivationText(item);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            ActivatablesCustomEditor.PaintBehaviors(item);

            return item;
        }

        public static Item PaintItemObjectSlot(Item item)
        {
            item = (Item)EditorGUILayout.ObjectField(item, typeof(Item), false);

            return item;
        }

        public static List<Item> PaintSkillObjectList(List<Item> itemList)
        {
            int size = Mathf.Max(0, EditorGUILayout.IntField("Skill Count", itemList.Count));

            while (size > itemList.Count)
            {
                itemList.Add(null);
            }

            while (size < itemList.Count)
            {
                itemList.RemoveAt(itemList.Count - 1);
            }

            EditorGUI.indentLevel++;
            for (int i = 0; i < itemList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Skill " + (i + 1), GUILayout.MaxWidth(100));

                Item item = itemList[i];
                item = PaintItemObjectSlot(item);
                itemList[i] = item;
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;

            return itemList;
        }
    }
#endif
    #endregion
}

public class ItemInfo : ActivatableInfo
{
    public Item item                    { get; private set; }
    public int amount                   { get; private set; }

    public void CheckActivatable()
    {
        activatable = ActivatableInfo.CheckActivatable(item);
    }
    public override void SetDeactivated()
    {
        base.SetDeactivated();
        if (item.hasPassive)
        {
            BattleManager.battleLog.LogBattleEffect($"{item.name} deactivated for {character.name}.");
        }
    }
    public void SetItem(Item item)
    {
        this.item = item;
    }

    #region CustomEditor
#if UNITY_EDITOR
    [CustomEditor(typeof(ItemInfo))]
    public class ItemInfoCustomEditor : Editor
    {
        public static ItemInfo PaintItemInfo(ItemInfo targetInfo)
        {
            ItemInfo info = targetInfo;

            EditorGUILayout.BeginHorizontal();
            info.item = Item.ItemCustomEditor.PaintItemObjectSlot(info.item);
            if (info.item != null)
            {
                info.showInfo = EditorGUILayout.Foldout(info.showInfo, "ExtraInfo", true);
            }
            EditorGUILayout.EndHorizontal();

            if (info.showInfo && info.item != null)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField("Editor Modifyable", EditorStyles.boldLabel);
                info.equipped = EditorGUILayout.Toggle("Equipped", info.equipped);
                info.activatable = EditorGUILayout.Toggle("Activatable", info.activatable);

                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel++;
            EditorGUILayout.Space();

            EditorGUI.indentLevel--;

            return info;
        }

        public static List<ItemInfo> PaintItemInfoList(Character character, List<ItemInfo> itemList)
        {
            int size = Mathf.Max(0, EditorGUILayout.IntField("Item Count", itemList.Count));

            while (size > itemList.Count)
            {
                itemList.Add(null);
            }

            while (size < itemList.Count)
            {
                itemList.RemoveAt(itemList.Count - 1);
            }

            EditorGUI.indentLevel++;
            for (int i = 0; i < itemList.Count; i++)
            {
                ItemInfo itemInfo = itemList[i];

                if (itemInfo == null)
                {
                    itemInfo = CreateInstance<ItemInfo>();
                    itemInfo.character = character;
                }

                itemInfo = PaintItemInfo(itemInfo);
                itemList[i] = itemInfo;
            }
            EditorGUI.indentLevel--;

            return itemList;
        }
    }
#endif
    #endregion
}