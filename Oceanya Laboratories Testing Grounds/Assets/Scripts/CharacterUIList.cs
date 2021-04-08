using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CharacterUIList : MonoBehaviour
{
    public int maxChars;
    public int curCharCount;

    public GameObject panel, objectUI;
    public ToggleGroup toggleGroup;
    public Toggle[] toggles;

    /// <summary>
    /// The character that is currently selected in the toggle group
    /// </summary>
    public static PlayerCharacter curCharacterSelected;

    [SerializeField]
    public List<BattleUI> charList = new List<BattleUI>();

    /// <summary>
    /// Creates and adds a character to the ally battle UIs.
    /// </summary>
    /// <param name="character">Character to add</param>
    public void AddChar(PlayerCharacter character)
    {
        if (charList.Count >= maxChars)
        {
            Destroy(charList[0].gameObject);
            charList.Remove(charList[0]);
            BattleManager.battleLog.LogBattleEffect($"Party is full! (Max: {maxChars}) Deleted first party member.");
        }

        GameObject newCharObject = Instantiate(objectUI, panel.transform);
        newCharObject.GetComponentInChildren<Toggle>().group = toggleGroup;


        BattleUI newCharUI;
        newCharUI = newCharObject.GetComponent<BattleUI>();
        newCharUI.LoadPlayerCharacter(character);
        

        charList.Add(newCharUI);
        curCharCount = charList.Count;
    }

    /// <summary>
    /// Selects a character from the toggle group
    /// </summary>
    /// <param name="character">Character to select</param>
    public void CurrentSelection(PlayerCharacter character)
    {
        curCharacterSelected = character;
        for (int i = 0; i < charList.Count; i++)
        {
            if (charList[i].loadedChar.ID == character.ID)
            {
                charList[i].GetComponentInChildren<Toggle>().isOn = true;
                break;
            }
        }
    }
}
