using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkillButton : MonoBehaviour
{
    public Skill loadedSkill;
    public Text buttonText;
    public Button button;

    public void LoadSkill(Skill skill)
    {
        gameObject.name = skill.baseInfo.name;
        buttonText.text = skill.baseInfo.name;
        loadedSkill = skill;

        if (skill.skillType == SkillType.Passive || CharacterActions.caster.activatedSkills.Contains(loadedSkill))
        {
            button.interactable = false;
        }
    }

    public void ActivateLoadedSkill()
    {
        loadedSkill.Activate(CharacterActions.caster, CharacterActions.target);
        CharacterActions.instance.skillToActivate = loadedSkill;

        if (CharacterActions.caster.activatedSkills.Contains(loadedSkill))
        {
            button.interactable = false;
        }
    }
}
