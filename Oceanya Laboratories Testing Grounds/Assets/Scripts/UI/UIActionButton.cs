using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIActionButton : MonoBehaviour
{
    public CharActions action;
    public TextMeshProUGUI text;
    ActionButtonToolTip tooltip;

    public void LoadAction(CharActions action)
    {
        tooltip = GetComponent<ActionButtonToolTip>();
        tooltip.info = UICharacterActions.GetActionDescription(action);
        this.action = action;

        gameObject.name = action.ToString();
        text.text = action.ToString();
    }

    public void ActivateLoadedAction()
    {
        UICharacterActions.instance.ButtonAction(action);
    }
}
