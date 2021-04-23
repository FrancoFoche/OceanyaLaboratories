using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkillContext : UIButtonScrollList
{
    static Character loadedCharacter;
    public static UISkillContext instance;

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
            AddSkill(character.skillList[i]);
        }
    }

    public void AddSkill(Skill skill)
    {
        GameObject newEntry = AddObject();
        newEntry.GetComponent<UISkillButton>().LoadSkill(skill);
        buttons.Add(newEntry.GetComponent<Button>());
    }

    public void Show()
    {
        LoadSkills(BattleManager.caster);
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
        ClearList();
    }
}
