using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public class DBItems : MonoBehaviour
{
    public static List<Item> items = new List<Item>();

    public static void BuildDatabase()
    {
        items = new List<Item>()
        {
            new Item(new BaseObjectInfo("Heal test", 1, "Heals"),
            "test",
            Item.Type.Consumable,
            GameAssetsManager.instance.GetItemIcon(ItemIcon.test1),
            ActivatableType.Active,
            TargetType.Single
            )
            .BehaviorDoesHeal(new List<RPGFormula>(){new RPGFormula(Stats.INT,operationActions.Multiply,2f)})
            .BehaviorCostsTurn()

        };

    }

}