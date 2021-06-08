using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIActionConfirmationPopUp : MonoBehaviour
{
    public GameObject objToHide;
    private Action confirmAction;

    private void Awake()
    {
        Hide();
    }

    public void Show(Action confirmAction, bool affectedByConfirmActionSetting, bool instant)
    {
        this.confirmAction = confirmAction;

        if (instant)
        {
            Confirm();
        }
        else
        {
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
    }
    public void Hide()
    {
        objToHide.SetActive(false);
    }
    public void Confirm()
    {
        Hide();
        confirmAction();
    }
    public void Deny()
    {
        Hide();
        if(!BattleManager.i.pauseMenu.paused)
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
}

public struct ActionData
{
    public Character caster;
    public List<Character> targets;

    public ActionData(Character caster, List<Character> targets)
    {
        this.caster = caster;
        this.targets = targets;
    }
}
