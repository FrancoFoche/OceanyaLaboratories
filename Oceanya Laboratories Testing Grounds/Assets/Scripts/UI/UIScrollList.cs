using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class UIScrollList : MonoBehaviour
{
    public int maxListCount;
    public int curListCount;

    public GameObject panel, objectUI;

    public List<GameObject> list = new List<GameObject>();

    public GameObject AddObject()
    {
        if (list.Count >= maxListCount)
        {
            Destroy(list[0].gameObject);
            list.Remove(list[0]);
        }

        GameObject newObject = Instantiate(objectUI, panel.transform);

        list.Add(newObject);
        curListCount = list.Count;

        return newObject;
    }

    public void ClearList()
    {
        for (int i = 0; i < list.Count; i++)
        {
            Destroy(list[i]);
        }
    }
}

public class UIToggleScrollList : UIScrollList
{
    public static List<GameObject> curObjectsSelected = new List<GameObject>();
    public ToggleGroup toggleGroup;
    public List<Toggle> toggles;
    public bool different;

    private void Update()
    {
        CheckCurrentSelection();
    }

    /// <summary>
    /// Checks which objects are currently selected
    /// </summary>
    public void CheckCurrentSelection()
    {
        different = false;
        List<GameObject>  newSelection = new List<GameObject>();

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].GetComponentInChildren<Toggle>().isOn)
            {
                newSelection.Add(list[i]);
                break;
            }
        }

        for (int i = 0; i < newSelection.Count; i++)
        {
            if(i < curObjectsSelected.Count)
            {
                if (newSelection[i] != curObjectsSelected[i])
                {
                    curObjectsSelected = newSelection;
                    different = true;
                    break;
                }
            }
            else
            {
                curObjectsSelected = newSelection;
                different = true;
            }
        }
    }

    /// <summary>
    /// Selects an object from the toggle group
    /// </summary>
    public void SelectObject(GameObject obj)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == obj)
            {
                list[i].GetComponentInChildren<Toggle>().isOn = true;
                break;
            }
        }
    }
    public void SelectObject(int index)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (i == index)
            {
                list[i].GetComponentInChildren<Toggle>().isOn = true;
                break;
            }
        }
    }
    public void TurnToggles(bool toggle)
    {
        for (int i = 0; i < toggles.Count; i++)
        {
            toggles[i].isOn = toggle;
        }
    }
    public void TurnToggleGroup(bool toggle)
    {
        if (toggle)
        {
            for (int i = 0; i < toggles.Count; i++)
            {
                toggles[i].group = toggleGroup;
            }
        }
        else
        {
            for (int i = 0; i < toggles.Count; i++)
            {
                toggles[i].group = null;
            }
        }

    }
}
