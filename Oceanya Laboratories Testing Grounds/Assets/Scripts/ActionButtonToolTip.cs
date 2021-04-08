using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Kam.TooltipUI;
using System.Text;

public class ActionButtonToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TooltipPopup tooltipPopup;
    [SerializeField] private string info = "";
    private void Start()
    {
        tooltipPopup = FindObjectOfType<TooltipPopup>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StringBuilder info = new StringBuilder();

        info.Append("<color=green>").Append("<size=15>").Append(gameObject.name).Append("</size>").Append("</color>").AppendLine();
        info.Append(this.info);
        tooltipPopup.DisplayInfo(info);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipPopup.HideInfo();
    }
}
