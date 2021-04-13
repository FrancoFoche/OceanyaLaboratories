using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Kam.TooltipUI;
using System.Text;

public class CharacterStatToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TooltipPopup tooltipPopup;
    public PlayerCharacter loadedChar;
    StringBuilder characterStats = new StringBuilder();

    private void Start()
    {
        tooltipPopup = FindObjectOfType<TooltipPopup>();
    }

    public void LoadCharStats(PlayerCharacter character)
    {
        loadedChar = character;
        characterStats = new StringBuilder();

        characterStats.Append("<size=15><color=green>").Append(character.name).Append("</color></size>").AppendLine().AppendLine();

        for (int i = 0; i < RuleManager.StatHelper.Count; i++)
        {
            Stats curStat = RuleManager.StatHelper[i];

            characterStats.Append("<size=15><color=green>").Append(curStat.ToString()).Append("</color></size>").Append("  |  ").Append(loadedChar.stats[curStat]).AppendLine();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipPopup.DisplayInfo(characterStats);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipPopup.HideInfo();
    }
}
