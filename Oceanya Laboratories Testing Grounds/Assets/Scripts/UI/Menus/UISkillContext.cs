using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkillContext : ButtonList
{
    public Color selectedColor;
    static Character loadedCharacter;
    public static UISkillContext instance;
    private List<UISkillButton> uiList = new List<UISkillButton>();
    private void Awake()
    {
        instance = this;
        Hide();
    }

    public void LoadSkills(Character character)
    {
        ClearList();
        buttons = new List<Button>();
        loadedCharacter = character;
        for (int i = 0; i < character.skillList.Count; i++)
        {
            AddSkill(character.skillList[i].skill);
        }
    }

    public void AddSkill(Skill skill)
    {
        GameObject newEntry = AddObject();
        buttons.Add(newEntry.GetComponent<Button>());

        UISkillButton newButton = newEntry.GetComponent<UISkillButton>();
        uiList.Add(newButton);
        newButton.LoadSkill(skill);
    }

    public void Show()
    {
        Character character = BattleManager.caster;
        LoadSkills(character);
        DeactivateVisualSelect();
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        BattleManager.i.tooltipPopup.HideInfo();
        gameObject.SetActive(false);
    }

    public override void InteractableButtons(bool state)
    {
        base.InteractableButtons(state);

        if (state && gameObject.activeSelf)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i].GetComponent<UISkillButton>().UpdateFormat();
            }
        }
    }

    public void RefreshFormats()
    {
        for (int i = 0; i < uiList.Count; i++)
        {
            uiList[i].UpdateFormat();
        }
    }

    public void VisualSelectButton(Skill skill)
    {
        for (int i = 0; i < list.Count; i++)
        {
            UISkillButton current = list[i].GetComponent<UISkillButton>();
            if (current.loadedSkill == skill)
            {
                current.ActivateColorOverlay(selectedColor);
            }
            else
            {
                current.DeactivateColorOverlay();
            }
        }
    }

    public void DeactivateVisualSelect()
    {
        for (int i = 0; i < list.Count; i++)
        {
            UISkillButton current = list[i].GetComponent<UISkillButton>();

            current.DeactivateColorOverlay();
        }
    }

    public UISkillButton GetSkillButton(Skill skill)
    {
        for (int i = 0; i < list.Count; i++)
        {
            UISkillButton current = list[i].GetComponent<UISkillButton>();
            if (current.loadedSkill == skill)
            {
                return current;
            }
        }

        throw new System.Exception("Skill button of " + skill.name + " skill could not be found.");
    }
    public void ActivateButtonInPosition(int position)
    {
        int index = position - 1;
        if (index < list.Count)
        {
            GameObject obj = list[index];
            UISkillButton current = obj.GetComponent<UISkillButton>();
            Button button = obj.GetComponent<Button>();

            if (button.interactable)
            {
                current.ActivateLoadedSkill();
            }
        }
    }
}
