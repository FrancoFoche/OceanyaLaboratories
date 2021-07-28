using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using Kam.TooltipUI;

public class LevelToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    StringBuilder levelDesc = new StringBuilder();
    public UILevelButton script;
    public TooltipPopup tooltip;

    private void Start()
    {
        tooltip = FindObjectOfType<TooltipPopup>();
    }

    public void ShowLevel(LevelManager.BattleLevel level)
    {
        levelDesc = new StringBuilder();

        levelDesc.Append("<size=15><color=green>").Append(level.name).Append("</color></size>").AppendLine().AppendLine();

        levelDesc.Append(level.description).AppendLine();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowLevel(script.level);
        tooltip.DisplayInfo(levelDesc);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.HideInfo();
    }
}