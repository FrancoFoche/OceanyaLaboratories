using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISkillButton : MonoBehaviour
{
    public Skill loadedSkill;
    public TextMeshProUGUI buttonText;
    public Button button;

    public void LoadSkill(Skill skill)
    {
        gameObject.name = skill.baseInfo.name;
        loadedSkill = skill;
        button = GetComponent<Button>();
        buttonText.text = CooldownStateFormatting(skill.baseInfo.name);
    }

    public void ActivateLoadedSkill()
    {
        loadedSkill.Activate(BattleManager.caster);
        UICharacterActions.instance.skillToActivate = loadedSkill;

        if (BattleManager.caster.activatedSkills.Contains(loadedSkill))
        {
            button.interactable = false;
        }
    }

    /// <summary>
    /// Checks the cooldown state of the skill and affects a button's properties to match that state
    /// </summary>
    public string CooldownStateFormatting(string text)
    {
        StringBuilder result = new StringBuilder();

        if (loadedSkill.skillType == SkillType.Passive)
        {
            result.Append("<color=#").Append(ColorUtility.ToHtmlStringRGB(Color.gray)).Append(">").Append(text).Append("</color>");

            if (BattleManager.caster.activatedSkills.Contains(loadedSkill))
            {
                button.interactable = false;
            }
        }
        else
        {
            if (BattleManager.caster.skillCooldowns.ContainsKey(loadedSkill))
            {
                switch (BattleManager.caster.skillCooldowns[loadedSkill])
                {
                    case CooldownStates.BeingUsed:
                        result.Append("<color=#").Append(ColorUtility.ToHtmlStringRGB(Color.green)).Append(">").Append(text).Append("</color>");
                        button.interactable = false;
                        break;
                    case CooldownStates.Usable:
                        result.Append("<color=#").Append(ColorUtility.ToHtmlStringRGB(Color.white)).Append(">").Append(text).Append("</color>");
                        button.interactable = true;
                        break;
                    case CooldownStates.OnCooldown:
                        result.Append("<color=#").Append(ColorUtility.ToHtmlStringRGB(Color.yellow)).Append(">").Append(text).Append(" (").Append(BattleManager.caster.GetCurrentCD(loadedSkill)+1).Append(" CD)").Append("</color>");
                        button.interactable = false;
                        break;
                    case CooldownStates.Used:
                        result.Append("<color=#").Append(ColorUtility.ToHtmlStringRGB(Color.gray)).Append(">").Append(text).Append("</color>");
                        button.interactable = false;
                        break;
                }
                
            }
            else
            {
                result.Append(text);
            }
        }

        return result.ToString();
    }
}
