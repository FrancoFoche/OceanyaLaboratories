using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Kam.TooltipUI;
using System.Text;

public class SkillToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Skill loadedSkill;
    StringBuilder result = new StringBuilder();

    public void LoadSkill(Skill skill)
    {
        loadedSkill = skill;
        result = new StringBuilder();

        result.Append("<size=15><color=green>").Append(skill.name).Append("</color></size>").AppendLine().AppendLine();
        result.Append(skill.description).AppendLine();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LoadSkill(gameObject.GetComponent<UISkillButton>().loadedSkill);
        BattleManager.i.tooltipPopup.DisplayInfo(result);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BattleManager.i.tooltipPopup.HideInfo();
    }
}
