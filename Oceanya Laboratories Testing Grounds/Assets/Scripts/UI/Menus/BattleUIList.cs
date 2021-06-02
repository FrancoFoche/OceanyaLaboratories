using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIList : ToggleList
{
    /// <summary>
    /// The character that is currently selected in the toggle group
    /// </summary>
    public static Character curCharacterSelected { get; private set; }

    [Header("Enemy Vars")]
    public GameObject enemyUI;
    public Transform enemyParent;

    private List<BattleUI> generalList = new List<BattleUI>();

    /// <summary>
    /// Creates and adds a player character to the ally battle UIs.
    /// </summary>
    /// <param name="character">Character to add</param>
    public AllyBattleUI         AddAlly         (Character character)   
    {
        GameObject newCharObject = AddObject();
        newCharObject.GetComponentInChildren<Toggle>().group = toggleGroup;


        AllyBattleUI newCharUI = newCharObject.GetComponent<AllyBattleUI>();
        character.curUI = newCharUI;

        toggles.Add(newCharUI.GetComponentInChildren<Toggle>());
        generalList.Add(newCharUI);

        System.Type type = character.GetType();
        if (type == typeof(PlayerCharacter))
        {
            newCharUI.LoadChar(GameAssetsManager.instance.GetPC(character.ID));
        }
        else if (type == typeof(Enemy))
        {
            newCharUI.LoadChar(GameAssetsManager.instance.GetEnemy(character.ID));
        }

        return newCharUI;
    }

    public EnemyBattleUI        AddEnemy        (Character character, Transform uiPosition)
    {
        Vector3 newPos = Camera.main.WorldToScreenPoint(uiPosition.position);

        GameObject newCharObject = AddObject(enemyUI, newPos, new Quaternion(0,0,0,0), enemyParent);
        newCharObject.GetComponentInChildren<Toggle>().group = toggleGroup;


        EnemyBattleUI newCharUI = newCharObject.GetComponent<EnemyBattleUI>();
        character.curUI = newCharUI;

        toggles.Add(newCharUI.GetComponentInChildren<Toggle>());
        generalList.Add(newCharUI);

        System.Type type = character.GetType();
        if (type == typeof(PlayerCharacter))
        {
            newCharUI.LoadChar(GameAssetsManager.instance.GetPC(character.ID));
        }
        else if (type == typeof(Enemy))
        {
            newCharUI.LoadChar(GameAssetsManager.instance.GetEnemy(character.ID));
        }

        return newCharUI;
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
                if(curCharacterSelected != character)
                {
                    Debug.Log($"Selected {character.name}");
                }

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

    public void             UpdateSelected  ()                      
    {
        if(curObjectsSelected.Count > 0)
        {
            curCharacterSelected = curObjectsSelected[0].GetComponent<BattleUI>().loadedChar;
        }
    }

    public void             SetTargettingMode(bool state)           
    {
        for (int i = 0; i < generalList.Count; i++)
        {
            generalList[i].TargettingMode(state);
        }
    }

    public void             InteractableUIs(bool state)
    {
        for (int i = 0; i < generalList.Count; i++)
        {
            generalList[i].InteractableUI(state);
        }

        Debug.Log("Set all UI's interactables to " + state);
    }
}
