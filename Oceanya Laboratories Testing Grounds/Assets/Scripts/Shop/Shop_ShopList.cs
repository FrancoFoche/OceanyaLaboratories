using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Kam.Shop
{
    public class Shop_ShopList : ButtonList
    {
        public TextMeshProUGUI headerText;

        private void Start()
        {
            DataBaseOrder.i.Initialize();
            LoadList(DBItems.items);
        }

        public void LoadList(List<Item> items)
        {
            ClearList();
            for (int i = 0; i < items.Count; i++)
            {
                Item current = items[i];
                GameObject newObj = AddObject();
                Shop_ShopLoader newScript = newObj.GetComponent<Shop_ShopLoader>();

                newScript.LoadShopItem(current);
            }
        }

        public void UpdateGoldAmount()
        {
            headerText.text = $"Shop (Current Gold: {SettingsManager.groupGold}G)";
        }

        public void ErrorAnimation()
        {

        }
    }
}