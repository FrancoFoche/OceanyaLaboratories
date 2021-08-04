using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Kam.Shop
{
    public class Shop_ShopLoader : MonoBehaviour
    {
        public Item loadedItem;

        public Image icon;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI costText;

        UIActionConfirmationPopUp popup;

        private void Start()
        {
            popup = FindObjectOfType<UIActionConfirmationPopUp>();
        }

        public void LoadShopItem(Item item)
        {
            icon.sprite = item.icon;
            nameText.text = item.name;
            costText.text = item.cost.ToString();

            loadedItem = item;
        }

        public void ButtonAction()
        {
            popup.Show(delegate { Shop_CharacterList.currentlySelected.GiveItem(loadedItem, 1); Shop_CharacterList.i.UpdateInventory(); },
                false,
                "Are you sure you want to buy a " + loadedItem.name + " for " + Shop_CharacterList.currentlySelected.name
                );
        }
    }
}
