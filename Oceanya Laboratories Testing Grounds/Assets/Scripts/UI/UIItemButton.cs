using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIItemButton : MonoBehaviour
{
    public Item loadedItem;
    public Button button;

    public TextMeshProUGUI itemName;
    public Image itemIcon;
    public TextMeshProUGUI itemAmount;

    public void LoadItem(Item item, int amount)
    {
        gameObject.name = item.name;
        loadedItem = item;
        itemIcon.sprite = item.icon;
        UpdateFormat();
    }
    public void ActivateLoadedItem()
    {
        if (loadedItem.targetType == TargetType.Multiple || loadedItem.targetType == TargetType.Single)
        {
            UICharacterActions.instance.SetItemToActivate(loadedItem);
        }
        else
        {
            UICharacterActions.instance.StartButtonActionConfirmation(() => UICharacterActions.instance.SetItemToActivate(loadedItem));
        }
    }
    public void UpdateFormat()
    {
        ItemInfo info = BattleManager.caster.GetItemFromInventory(loadedItem);
        itemName.text = info.item.name;
        itemAmount.text = "x" + info.amount.ToString();
    }
}