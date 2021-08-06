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
            Shop_CharacterList.i.shop.UpdateGoldAmount();
        }

        public void LoadShopItem(Item item)
        {
            icon.sprite = item.icon;
            nameText.text = item.name;
            costText.text = item.cost.ToString() + "G";

            loadedItem = item;
        }

        public void ButtonAction()
        {
            if(Shop_CharacterList.currentlySelected != null)
            {
                if (SettingsManager.groupGold >= loadedItem.cost)
                {
                    popup.Show(
                    delegate
                    {
                        SettingsManager.groupGold -= loadedItem.cost;
                        Shop_CharacterList.currentlySelected.GiveItem(loadedItem, 1);
                        Shop_CharacterList.i.UpdateInventory();
                        Shop_CharacterList.i.shop.UpdateGoldAmount();
                    },
                    true,
                    "Are you sure you want to buy a " + loadedItem.name + " for " + Shop_CharacterList.currentlySelected.name + "?"
                    );
                }
            }
            else
            {
                Shop_CharacterList.i.ErrorAnimation();
            }
        }
    }
}
