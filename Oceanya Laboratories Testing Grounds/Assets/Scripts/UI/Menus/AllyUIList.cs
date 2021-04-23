using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllyUIList : UIToggleScrollList
{
    /// <summary>
    /// The character that is currently selected in the toggle group
    /// </summary>
    public static Character curCharacterSelected;

    /// <summary>
    /// Creates and adds a character to the ally battle UIs.
    /// </summary>
    /// <param name="character">Character to add</param>
    public void             AddChar         (Character character)   
    {
        GameObject newCharObject = AddObject();
        newCharObject.GetComponentInChildren<Toggle>().group = toggleGroup;


        BattleUI newCharUI;
        newCharUI = newCharObject.GetComponent<BattleUI>();
        newCharUI.LoadPlayerCharacter(character);


        toggles.Add(newCharUI.GetComponentInChildren<Toggle>());
    }

    /// <summary>
    /// Selects a character from the toggle group
    /// </summary>
    /// <param name="character">Character to select</param>
    public void             SelectCharacter (Character character)   
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].GetComponent<BattleUI>().loadedChar.ID == character.ID)
            {
                SelectObject(i);
                curCharacterSelected = character;
                break;
            }
        }

        UISkillContext.instance.Hide();
    }

    public List<Character>  CheckTargets    ()                      
    {
        List<Character> targetsOn = new List<Character>();

        for (int i = 0; i < toggles.Count; i++)
        {
            if (toggles[i].isOn)
            {
                targetsOn.Add(toggles[i].GetComponentInParent<BattleUI>().loadedChar);
            }
        }

        return targetsOn;
    }
}
