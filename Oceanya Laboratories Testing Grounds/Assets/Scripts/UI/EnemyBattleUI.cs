using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBattleUI : BattleUI
{
    public override void LoadChar(Character character)
    {
        base.LoadChar(character);

        System.Type type = character.GetType();

        if (type == typeof(PlayerCharacter))
        {
            this.type = CharacterType.PlayerCharacter;
        }
        else if (type == typeof(Enemy))
        {
            this.type = CharacterType.Enemy;
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
