using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public class DBItems : MonoBehaviour
{
    public static List<Item> items = new List<Item>();
    /*
    public static void buildDataBase()
    {
        items = new List<Item>()
        {
            new Item(new BaseObjectInfo("Heal test", 1, "Heals"), ItemType.consume, TargetType.Self,
            new Dictionary<Item.ItemStats, int>()
            {
                { Item.ItemStats.Heal, 20 }
            })  .BehaviorDoesHeal(),
            new Item(new BaseObjectInfo("Test 2", 2, "test"), ItemType.consume, TargetType.Self,
            new Dictionary<Item.ItemStats, int>())

        };
    }
    */
}