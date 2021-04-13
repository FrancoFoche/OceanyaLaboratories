using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct StatLine
{
    public Text statName;
    public Text baseStat;
    public Text resultStat;
}

public class BattleUI : MonoBehaviour
{
    [Header("Character Loaded")]
    public int                                      charID;
    public string                                   charName;

    public PlayerCharacter                          loadedChar;

    [Header("BASE INFO")]
    public Text                                     nameText;
    public Text                                     levelText;

    public Slider                                   hpSlider;
    public Text                                     hpText;

    public Text                                     classText;
    public Text                                     statusEffectText;
    public CharacterStatToolTip                     toolTip;


    private void Update()
    {
        if(loadedChar != null)
        {
            charName = loadedChar.name;
        }
        else
        {
            charName = "Does not exist";
        }
    }

    public void LoadPlayerCharacter(PlayerCharacter character)
    {
        loadedChar = character;
        charID = character.ID;

        nameText.text = character.name;
        levelText.text = "LV. " + character.level.ToString();

        hpSlider.minValue = 0;
        hpSlider.maxValue = character.stats[Stats.MAXHP];
        hpSlider.value = character.stats[Stats.CURHP];

        hpText.text = character.stats[Stats.CURHP] + " / " + character.stats[Stats.MAXHP];

        classText.text = character.rpgClass.baseInfo.name;
        statusEffectText.text = "None";

        toolTip.LoadCharStats(character);
    }

    public void UpdateUI()
    {
        LoadPlayerCharacter(loadedChar);
    }

    public void CheckSelection()
    {
        if (GetComponentInChildren<Toggle>().isOn)
        {
            CharacterUIList.curCharacterSelected = loadedChar;
        }
    }
}
