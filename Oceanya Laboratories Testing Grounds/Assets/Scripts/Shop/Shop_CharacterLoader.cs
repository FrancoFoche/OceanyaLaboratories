using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Kam.Shop
{
    public class Shop_CharacterLoader : MonoBehaviour
    {
        public TextMeshProUGUI newText;
        public PlayerCharacter loadedChar;
        public Image colorOverlay;

        public void LoadCharacter(PlayerCharacter character)
        {
            loadedChar = character;
            newText.text = character.name;
        }

        public void Select()
        {
            colorOverlay.gameObject.SetActive(true);
        }

        public void Unselect()
        {
            colorOverlay.gameObject.SetActive(false);
        }
    }
}
