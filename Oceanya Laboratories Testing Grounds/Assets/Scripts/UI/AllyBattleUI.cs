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

public class AllyBattleUI : BattleUI
{
    [Header("ALLY INFO")]
    public Text                                    levelText;

    public Text                                    classText;

    private void Update()
    {
        if(loadedChar != null)
        {
            charName = loadedChar.name;
        }
        else
        {
            charName = "Char does not exist";
        }
    }

    public override void LoadChar(Character character)
    {
        base.LoadChar(character);

        levelText.text = "LV. " + character.level.ToString();
        System.Type type = character.GetType();

        if (type == typeof(PlayerCharacter))
        {
            this.type = CharacterType.PlayerCharacter;
            classText.text = DBPlayerCharacter.GetPC(charID).rpgClass.baseInfo.name;
        }
        else if (type == typeof(Enemy))
        {
            this.type = CharacterType.Enemy;
            classText.text = "None";
        }
    }

    public override void UpdateUI()
    {
        switch (type)
        {
            case CharacterType.PlayerCharacter:
                LoadChar(DBPlayerCharacter.GetPC(charID));
                break;
            case CharacterType.Enemy:
                LoadChar(DBEnemies.GetEnemy(charID));
                break;
        }
    }
}
