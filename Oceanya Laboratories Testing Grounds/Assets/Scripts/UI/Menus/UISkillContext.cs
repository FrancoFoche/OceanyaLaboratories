using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkillContext : ButtonList
{
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
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        BattleManager.i.tooltipPopup.HideInfo();
        gameObject.SetActive(false);
    }

    public void InteractableUIButtons(bool state)
    {
        InteractableButtons(state);
    }

    public void RefreshFormats()
    {
        for (int i = 0; i < uiList.Count; i++)
        {
            uiList[i].UpdateFormat();
        }
    }
}
