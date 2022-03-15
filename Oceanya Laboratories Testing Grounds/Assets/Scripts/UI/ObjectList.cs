﻿using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class ObjectList : MonoBehaviour
{
    public int maxListCount;
    public int curListCount;

    public GameObject parent, obj;

    public List<GameObject> list = new List<GameObject>();

    public virtual GameObject AddObject()
    {
        if (list.Count >= maxListCount)
        {
            Destroy(list[0].gameObject);
            list.Remove(list[0]);
        }

        GameObject newObject = Instantiate(obj, parent.transform);

        list.Add(newObject);
        curListCount = list.Count;

        return newObject;
    }

    public virtual GameObject AddObject(Vector3 pos, Quaternion rot)
    {
        if (list.Count >= maxListCount)
        {
            Destroy(list[0].gameObject);
            list.Remove(list[0]);
        }

        GameObject newObject = Instantiate(obj, pos, rot);
        newObject.transform.parent = parent.transform;

        list.Add(newObject);
        curListCount = list.Count;

        return newObject;
    }

    public virtual GameObject AddObject(Vector3 pos, Quaternion rot, Transform parent)
    {
        if (list.Count >= maxListCount)
        {
            Destroy(list[0].gameObject);
            list.Remove(list[0]);
        }

        GameObject newObject = Instantiate(obj, pos, rot, parent);

        list.Add(newObject);
        curListCount = list.Count;

        return newObject;
    }

    public virtual GameObject AddObject(GameObject objToInstance)
    {
        if (list.Count >= maxListCount)
        {
            Destroy(list[0].gameObject);
            list.Remove(list[0]);
        }

        GameObject newObject = Instantiate(objToInstance, parent.transform);

        list.Add(newObject);
        curListCount = list.Count;

        return newObject;
    }

    public virtual GameObject AddObject(GameObject objToInstance, Vector3 pos, Quaternion rot, Transform parent)
    {
        if (list.Count >= maxListCount)
        {
            Destroy(list[0].gameObject);
            list.Remove(list[0]);
        }

        GameObject newObject = Instantiate(objToInstance, pos, rot, parent);

        list.Add(newObject);
        curListCount = list.Count;

        return newObject;
    }

    public virtual void Remove(GameObject obj)
    {
        if (list.Contains(obj))
        {
            list.Remove(obj);
            Destroy(obj);
        }
    }

    public virtual void ClearList()
    {
        for (int i = 0; i < list.Count; i++)
        {
            Destroy(list[i]);
        }
        list.Clear();
    }
}

public class ToggleList : ObjectList
{
    public static List<GameObject> curObjectsSelected = new List<GameObject>();
    public ToggleGroup toggleGroup;
    public List<Toggle> toggles = new List<Toggle>();
    public bool different { get; private set; }


    /// <summary>
    /// Checks which objects are currently selected
    /// </summary>
    public void CheckCurrentSelection   ()              
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

    #region AddObject Overrides
    public override GameObject AddObject()
    {
        GameObject obj = base.AddObject();
        ToggleAddToList(obj);
        return obj;
    }
    public override GameObject AddObject(GameObject objToInstance, Vector3 pos, Quaternion rot, Transform parent)
    {
        GameObject obj = base.AddObject(objToInstance, pos, rot, parent);
        ToggleAddToList(obj);
        return obj;
    }
    public override GameObject AddObject(Vector3 pos, Quaternion rot)
    {
        GameObject obj = base.AddObject(pos, rot);
        ToggleAddToList(obj);
        return obj;
    }
    public override GameObject AddObject(Vector3 pos, Quaternion rot, Transform parent)
    {
        GameObject obj = base.AddObject(pos, rot, parent);
        ToggleAddToList(obj);
        return obj;
    }
    void ToggleAddToList(GameObject obj)
    {
        Toggle toggle = obj.GetComponent<Toggle>();

        if(toggle == null)
        {
            toggle = obj.GetComponentInChildren<Toggle>();
        }

        toggle.group = toggleGroup == null ? null : toggleGroup;
        toggles.Add(toggle);
    }
    #endregion

    /// <summary>
    /// Selects an object from the toggle group
    /// </summary>
    public void SelectObject            (GameObject obj)
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
    public void SelectObject            (int index)     
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
    public void TurnToggles             (bool toggle)   
    {
        for (int i = 0; i < toggles.Count; i++)
        {
            toggles[i].isOn = toggle;
        }

        Debug.Log("Set all toggles to " + toggle);
    }
    public void TurnToggleGroup         (bool toggle)   
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
    public void InteractableToggles     (bool state)    
    {
        for (int i = 0; i < toggles.Count; i++)
        {
            toggles[i].interactable = state;
        }

        Debug.Log("Set all toggles' interactables to " + state);
    }
    public override void ClearList()
    {
        base.ClearList();
        toggles.Clear();
    }
}

public class ButtonList : ObjectList
{
    public List<Button> buttons;

    #region AddObject Overrides
    public override GameObject AddObject()
    {
        GameObject obj = base.AddObject();
        AddButtonToList(obj);
        return obj;
    }

    public override GameObject AddObject(GameObject objToInstance)
    {
        GameObject obj = base.AddObject(objToInstance);
        AddButtonToList(obj);
        return obj;
    }

    public override GameObject AddObject(GameObject objToInstance, Vector3 pos, Quaternion rot, Transform parent)
    {
        GameObject obj = base.AddObject(objToInstance, pos, rot, parent);
        AddButtonToList(obj);
        return obj;
    }
    public override GameObject AddObject(Vector3 pos, Quaternion rot)
    {
        GameObject obj = base.AddObject(pos, rot);
        AddButtonToList(obj);
        return obj;
    }
    public override GameObject AddObject(Vector3 pos, Quaternion rot, Transform parent)
    {
        GameObject obj = base.AddObject(pos, rot, parent);
        AddButtonToList(obj);
        return obj;
    }
    void AddButtonToList(GameObject obj)
    {
        Button button = obj.GetComponent<Button>();
        buttons.Add(button);
    }
    #endregion

    public virtual void InteractableButtons(bool state)
    {
        if (!state)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].interactable = false;
            }
        }
        else
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].interactable = true;
            }
        }
    }

    public override void ClearList()
    {
        base.ClearList();
        buttons.Clear();
    }
}
