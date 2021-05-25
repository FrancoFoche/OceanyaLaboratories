using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIItemButton : MonoBehaviour
{
    public Item loadedItem;
    public TextMeshProUGUI buttonText;
    public Button button;

    public void LoadItem(Item item)
    {
        gameObject.name = item.baseInfo.name;
        loadedItem = item;
        button = GetComponent<Button>();
        buttonText.text = loadedItem.baseInfo.name;
    }
    public void ActivateLoadedItem()
    {
        loadedItem.Activate(BattleManager.caster);
        UICharacterActions.instance.itemToUse = loadedItem;
    }

}