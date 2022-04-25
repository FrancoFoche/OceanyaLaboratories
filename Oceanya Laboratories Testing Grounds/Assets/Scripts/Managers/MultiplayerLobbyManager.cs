using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Linq;

public class MultiplayerLobbyManager : MonoBehaviourPun
{
    public static MultiplayerLobbyManager i;
    public static string username;
    public static bool owner = true;
    public static string lobbyName;
    public static int playerCount;

    public ObjectList playerList;
    public TextMeshProUGUI lobbyNameText;
    public UIMissionDropdown missionDropdown;
    public Button fightButton;
    public bool limitMinPlayers;

    

    private void Awake()
    {
        i = this;
    }

    void Start()
    {
        lobbyNameText.text = $"Lobby - \"{lobbyName}\"";
        playerCount = PhotonNetwork.PlayerList.Length;
        
        //If player count is 1, you are the owner, set interactable based on that.
        owner = playerCount == 1;
        if (owner)
        {
            fightButton.interactable = true;
            missionDropdown.dropdown.interactable = true;
        }
        else
        {
            fightButton.interactable = false;
            missionDropdown.dropdown.interactable = false;
        }
        
        
        
        //Spawn a player prefab and load the information into the script
        InstantiateItem();
    }
    private void Update()
    {
        playerCount = PhotonNetwork.PlayerList.Length;
        if (owner)
        {
            if (limitMinPlayers)
            {
                if (playerCount >= 2)
                {
                    fightButton.interactable = true;
                }
                else
                {
                    fightButton.interactable = false;
                }
            }
            else
            {
                fightButton.interactable = true;
            }
        }
    }

    void InstantiateItem()
    {
        //Instantiates an object online.
        GameObject obj = playerList.AddObject();

        //Initialize it (for everyone)
        UIMultiplayerLobbyList script = obj.GetComponent<UIMultiplayerLobbyList>();
        script.Initialize(username, playerList.parent, false, owner);

        LevelManager.CreateLevels();
        missionDropdown.SetLevels(LevelManager.levels.ToList());
    }

    public void FightButton()
    {
        photonView.RPC(nameof(FightButtonOnline), RpcTarget.All);
    }

    [PunRPC]
    void FightButtonOnline()
    {
        //Set local player's settings
        MultiplayerBattleManager.players = playerList.list.Select(x => x.GetComponent<UIMultiplayerLobbyList>().Confirm()).ToArray();
        Debug.Log("Players count RPC: " + MultiplayerBattleManager.players.Length);
        MultiplayerBattleManager.multiplayerActive = true;

        //Set mission selected to level manager
        LevelManager.currentLevel = missionDropdown.Selected.levelNumber;

        //Play
        SceneLoaderManager.instance.LoadPlay();
    }
}
