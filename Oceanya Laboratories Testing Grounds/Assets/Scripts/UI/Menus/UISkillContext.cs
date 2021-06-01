﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkillContext : ButtonList
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
            if(character.skillList[i] != null)
            {
                if (character.skillList[i].skill != null)
                {
                    AddSkill(character.skillList[i].skill);
                }
            }
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
        Character character = BattleManager.caster;
        LoadSkills(character);
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
