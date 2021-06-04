using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIActionConfirmationPopUp : MonoBehaviour
{
    public GameObject objToHide;
    private Character caster;
    private List<Character> targets;
    private Action<Character, List<Character>> actionType1;
    private Action actionType2;
    private actionTypes type;
    public enum actionTypes
    {
        CasterAndTargets,
        NoParameters
    }
    private void Awake()
    {
        Hide();
    }

    public void SetCharacters(Character caster, List<Character> targets)
    {
        this.caster = caster;
        this.targets = targets;
    }

    public void SetConfirmAction(Action<Character, List<Character>> confirmAction)
    {
        actionType1 = confirmAction;
        type = actionTypes.CasterAndTargets;
    }
    public void SetConfirmAction(Action action)
    {
        actionType2 = action;
        type = actionTypes.NoParameters;
    }
    public void Show(Character caster, List<Character> targets, Action<Character, List<Character>> confirmAction, bool affectedByConfirmActionSetting = true)
    {
        SetCharacters(caster, targets);
        SetConfirmAction(confirmAction);
        if (affectedByConfirmActionSetting)
        {
            if (BattleManager.i.confirmMode)
            {
                objToHide.SetActive(true);
            }
            else
            {
                Confirm();
            }
        }
        else
        {
            objToHide.SetActive(true);
        }
    }

    public void Show(Action action, bool affectedByConfirmActionSetting = true)
    {
        SetConfirmAction(action);
        if (affectedByConfirmActionSetting)
        {
            if (BattleManager.i.confirmMode)
            {
                objToHide.SetActive(true);
            }
            else
            {
                Confirm();
            }
        }
        else
        {
            objToHide.SetActive(true);
        }
    }

    public void Hide()
    {
        objToHide.SetActive(false);
    }

    public void Confirm()
    {
        Hide();
        InvokeAction(type);
    }
    public void Deny()
    {
        Hide();
        if(type == actionTypes.CasterAndTargets)
        {
            BattleManager.i.battleLog.LogBattleEffect("Cancelled Action.");
            TeamOrderManager.SetTurnState(TurnState.WaitingForAction);

            Character character;

            if (BattleManager.i.debugMode)
            {
                character = BattleManager.caster;
            }
            else
            {
                character = TeamOrderManager.currentTurn;
            }

            BattleManager.i.uiList.SelectCharacter(character);
        }
    }

    public void InvokeAction(actionTypes type)
    {
        switch (type)
        {
            case actionTypes.CasterAndTargets:
                actionType1.Invoke(caster, targets);
                break;
            case actionTypes.NoParameters:
                actionType2.Invoke();
                break;
            default:
                break;
        }
    }
}
