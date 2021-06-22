using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Kam.TooltipUI;
using System.Text;

public class CharacterStatToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Character loadedChar;
    StringBuilder characterStats = new StringBuilder();

    public void LoadCharStats(Character character)
    {
        loadedChar = character;
        characterStats = new StringBuilder();

        characterStats.Append("<size=15><color=green>").Append(character.name).Append("</color></size>").AppendLine().AppendLine();

        for (int i = 0; i < RuleManager.StatHelper.Length; i++)
        {
            Stats curStat = RuleManager.StatHelper[i];

            characterStats.Append("<size=15><color=green>").Append(curStat.ToString()).Append("</color></size>").Append("  |  ").Append(loadedChar.stats.GetStat(curStat).value).AppendLine();
        }

        characterStats.AppendLine().Append("<size=15><color=green>").Append("Basic Attack: ").Append("</color></size>")
            .Append(RPGFormula.FormulaListToString(character.basicAttackFormula)).Append(" (").Append(character.basicAttackType.ToString()).Append(" DMG)").AppendLine();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LoadCharStats(loadedChar);
        BattleManager.i.tooltipPopup.DisplayInfo(characterStats);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BattleManager.i.tooltipPopup.HideInfo();
    }
}
