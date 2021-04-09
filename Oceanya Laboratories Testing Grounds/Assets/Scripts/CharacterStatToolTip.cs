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

    public static Dictionary<int, Character.Stats> statDictionary =
    new Dictionary<int, Character.Stats>
    {
            { 0, Character.Stats.STR },
            { 1, Character.Stats.INT },
            { 2, Character.Stats.CHR },
            { 3, Character.Stats.AGI },
            { 4, Character.Stats.MR },
            { 5, Character.Stats.PR },
            { 6, Character.Stats.CON },
            { 7, Character.Stats.HPREGEN }
    };

    private void Start()
    {
        tooltipPopup = FindObjectOfType<TooltipPopup>();
    }

    public void LoadCharStats(PlayerCharacter character)
    {
        loadedChar = character;
        characterStats = new StringBuilder();

        characterStats.Append("<size=15><color=green>").Append(character.name).Append("</color></size>").AppendLine().AppendLine();

        for (int i = 0; i < statDictionary.Count; i++)
        {
            Character.Stats curStat = statDictionary[i];

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
