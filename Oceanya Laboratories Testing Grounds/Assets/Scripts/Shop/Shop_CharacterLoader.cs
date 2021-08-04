using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Kam.Shop
{
    public class Shop_CharacterLoader : MonoBehaviour
    {
        public TextMeshProUGUI newText;
        public PlayerCharacter loadedChar;

        public void LoadCharacter(PlayerCharacter character)
        {
            loadedChar = character;
            newText.text = character.name;
        }
    }
}
