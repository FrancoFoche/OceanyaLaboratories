using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIActionConfirmationPopUp : MonoBehaviour
{
    public GameObject objToHide;
    private Character caster;
    private List<Character> targets;
    private void Awake()
    {
        Hide();
    }

    public void Show(Character caster, List<Character> targets)
    {
        this.caster = caster;
        this.targets = targets;
        objToHide.SetActive(true);
    }

    public void Hide()
    {
        objToHide.SetActive(false);
    }

    public void Confirm()
    {
        Hide();
        UICharacterActions.instance.Act(caster, targets);
    }
    public void Deny()
    {
        Hide();
        TeamOrderManager.SetTurnState(TurnState.WaitingForAction);
    }
}
