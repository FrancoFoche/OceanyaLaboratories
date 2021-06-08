using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Kam.TooltipUI;
using System.Text;

public class ActionButtonToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string info = "";

    public void OnPointerEnter(PointerEventData eventData)
    {
        StringBuilder info = new StringBuilder();

        info.Append("<color=green>").Append("<size=15>").Append(gameObject.name).Append("</size>").Append("</color>").AppendLine();
        info.Append(this.info);
        BattleManager.i.tooltipPopup.DisplayInfo(info);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BattleManager.i.tooltipPopup.HideInfo();
    }
}
