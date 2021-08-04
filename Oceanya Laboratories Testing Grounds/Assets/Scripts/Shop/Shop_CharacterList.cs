using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Kam.Shop
{
    public class Shop_CharacterList : ToggleList
    {
        public static Shop_CharacterList i;
        public Shop_InventoryList inventory;

        public static PlayerCharacter currentlySelected;

        private void Awake()
        {
            i = this;
        }

        private void Start()
        {
            DataBaseOrder.i.Initialize();
            LoadList(DBPlayerCharacter.pCharacters);
        }

        private void Update()
        {
            CheckCurrentSelection();

            if (different)
            {
                GameObject newObj = curObjectsSelected[0].gameObject;
                Shop_CharacterLoader newScript = newObj.GetComponent<Shop_CharacterLoader>();
                currentlySelected = newScript.loadedChar;
                Debug.Log("Selected: " + currentlySelected.name);

                inventory.headerText.text = currentlySelected.name + "'s Inventory";
                inventory.LoadItems(currentlySelected);
            }
        }

        public void LoadList(List<PlayerCharacter> list)
        {
            ClearList();
            for (int i = 0; i < list.Count; i++)
            {
                PlayerCharacter current = list[i];
                if(current.name != "TestDummy")
                {
                    GameObject newObj = AddObject();
                    Shop_CharacterLoader newScript = newObj.GetComponent<Shop_CharacterLoader>();

                    newScript.LoadCharacter(current);
                }
            }
        }
        public void UpdateInventory()
        {
            inventory.LoadItems(currentlySelected);
        }
    }
}