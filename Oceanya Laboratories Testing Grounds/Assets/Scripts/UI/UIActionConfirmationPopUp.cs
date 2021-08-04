﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class UIActionConfirmationPopUp : MonoBehaviour
{
    public GameObject objToHide;
    public TextMeshProUGUI confirmationText;
    public TextMeshProUGUI yesText;
    public TextMeshProUGUI noText;
    private Action confirmAction;
    public bool waitingForConfirmation;

    private void Awake()
    {
        Hide();
    }

    public void Show(Action confirmAction, bool affectedByConfirmActionSetting, string detailText = "Are you sure you want to commit this action?", string yesText = "Yes", string noText = "No")
    {
        this.confirmAction = confirmAction;
        waitingForConfirmation = true;
        TeamOrderManager.turnState = TurnState.WaitingForConfirmation;
        confirmationText.text = detailText;
        this.yesText.text = yesText;
        this.noText.text = noText;

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
        waitingForConfirmation = false;
        Hide();
        confirmAction();
    }
    public void Deny()
    {
        waitingForConfirmation = false;
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
