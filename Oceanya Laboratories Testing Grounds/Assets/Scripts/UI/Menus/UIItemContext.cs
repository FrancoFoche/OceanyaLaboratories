using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemContext : ButtonList
{
    static Character loadedCharacter;
    public static UIItemContext instance;

    private void Awake()
    {
        instance = this;
        Hide();
    }

    public void LoadItems(Character character)
    {
        ClearList();
        buttons = new List<Button>();
        loadedCharacter = character;
        for (int a = 0; a < character.inventory.Count; a++)
        {
            AddItem(character.inventory[a]);
        }
    }

    public void AddItem(ItemInfo item)
    {
        GameObject newEntry = AddObject(); ;
        newEntry.GetComponent<UIItemButton>().LoadItem(item.item, item.amount);
        buttons.Add(newEntry.GetComponent<Button>());
    }

    public void Show()
    {
        Character character = BattleManager.caster;
        LoadItems(character);
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}