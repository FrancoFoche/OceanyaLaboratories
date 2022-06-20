using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Linq;

public class MultiplayerLobbyManager : MonoBehaviourPun
{
    public static MultiplayerLobbyManager i;
    public static string username;
    public static bool serverHost = true;
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
        serverHost = playerCount == 1;
        if (serverHost)
        {
            photonView.RPC(nameof(SetServerHost), RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
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
        if (serverHost)
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
        script.Initialize(username, playerList.parent, false, serverHost);

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
        List<PlayerCharacter> alreadySelected = new List<PlayerCharacter>();
        bool ableToFight = true;
        foreach (var player in playerList.list)
        {
            UIMultiplayerLobbyList ui = player.GetComponent<UIMultiplayerLobbyList>();
            PlayerCharacter selected = ui.playerCharacter.Selected;
            if (alreadySelected.Contains(selected))
            {
                ableToFight = false;
            }
            
            alreadySelected.Add(selected);
        }

        if (ableToFight)
        {
            //Set local player's settings
            Multiplayer_Server.players = playerList.list.Select(x => x.GetComponent<UIMultiplayerLobbyList>().Confirm()).ToArray();
            Debug.Log("Players count RPC: " + Multiplayer_Server.players.Length);
            Multiplayer_Server.multiplayerActive = true;

            //Set mission selected to level manager
            LevelManager.currentLevel = missionDropdown.Selected.levelNumber;

            //Play
            SceneLoaderManager.instance.LoadPlay();
        }
        else
        {
            StartCoroutine(ErrorCoroutine());
        }
    }

    IEnumerator ErrorCoroutine()
    {
        string text = lobbyNameText.text;

        lobbyNameText.text = "<color=red>You need to all select different characters!</color>";

        yield return new WaitForSeconds(2f);
        
        lobbyNameText.text = text;
    }
    [PunRPC]
    void SetServerHost(Player host)
    {
        Multiplayer_Server.serverHost = host;
    }
}
