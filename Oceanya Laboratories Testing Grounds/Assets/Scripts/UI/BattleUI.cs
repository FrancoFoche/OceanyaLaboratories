﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class BattleUI : MonoBehaviour, ILoader<Character>, IPunObservable
{
    [Header("Character Loaded")]
    [SerializeField] protected int charHashCode;
    [SerializeField] protected CharacterType type;
    [SerializeField] protected int charID;
    [SerializeField] protected string charName;
    [SerializeField] protected BattleUI curUI;
    [SerializeField] protected int charHP;
    [SerializeField] protected int charSTR;
    [SerializeField] protected Character _loadedChar;
    public Character loadedChar { get { return _loadedChar; } private set { _loadedChar = value; } }

    [Header("BASE INFO")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI statusEffectText;

    public Slider hpSlider;

    public CharacterStatToolTip toolTip;
    public EffectAnimator effectAnimator;
    public Image targettingModeImage;
    public Image raycastBlock;

    private void Update()
    {
        /*
        if(loadedChar != null)
        {
            #region Inspector Update, just for testing.
            charName = loadedChar.name;
            charID = loadedChar.ID;
            curUI = loadedChar.curUI;
            charHP = loadedChar.GetStat(Stats.CURHP).value;
            charSTR = loadedChar.GetStat(Stats.STR).value;
            charHashCode = loadedChar.GetHashCode();
            #endregion
        }*/
    }

    public virtual void LoadChar(Character character)
    {
        loadedChar = character;
        charID = character.ID;
        nameText.text = character.name;
        hpSlider.minValue = 0;
        hpSlider.maxValue = character.GetStat(Stats.MAXHP).value;
        hpSlider.value = character.GetStat(Stats.CURHP).value;

        hpText.text = character.GetStat(Stats.CURHP).value + " / " + character.GetStat(Stats.MAXHP).value;

        statusEffectText.text = "None";

        toolTip.LoadCharStats(character);
    }

    public virtual void OverrideCharacterWithPlayer(UIMultiplayerLobbyList.Settings player)
    {
        nameText.text = player.nickname;
    }
    public virtual void UpdateUI()
    {
        
    }

    public void TargettingMode(bool state)
    {
        targettingModeImage.gameObject.SetActive(state);
    }

    public void InteractableUI(bool state)
    {
        raycastBlock.gameObject.SetActive(!state);
    }

    public Character GetLoaded()
    {
        return _loadedChar;
    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //if (!MultiplayerLobbyManager.serverHost) return;
            //Write to network

            stream.SendNext(nameText.text);
            stream.SendNext(hpText.text);
            stream.SendNext(statusEffectText.text);
            stream.SendNext(hpSlider.value);

            #region Sync Character
            foreach (Stats stat in RuleManager.StatHelper)
            {
                stream.SendNext(_loadedChar.stats.GetStat(stat).value);
            }
        
            stream.SendNext(_loadedChar.dead);
            #endregion
        }
        else
        {
            //Get from network

            nameText.text = (string)stream.ReceiveNext();
            hpText.text = (string)stream.ReceiveNext();
            statusEffectText.text = (string)stream.ReceiveNext();
            hpSlider.value = (float)stream.ReceiveNext();
        
            foreach (Stats stat in RuleManager.StatHelper)
            {
                _loadedChar.stats.GetStat(stat).value = (int)stream.ReceiveNext();
            }
        
            _loadedChar.dead = (bool)stream.ReceiveNext();
            UpdateUI();
            BattleManager.i.UpdateTeamOrder();
        }
        
    }
    
}
