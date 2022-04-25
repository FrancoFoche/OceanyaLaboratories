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
    public static string username;
    public static string lobbyName;
    public static int playerCount;

    public ObjectList playerList;
    public TextMeshProUGUI lobbyNameText;
    public UIMissionDropdown missionDropdown;
    public Button fightButton;
    public bool limitMinPlayers;


    void Start()
    {
        lobbyNameText.text = $"Lobby - \"{lobbyName}\"";

        //Spawn a player prefab and load the information into the script
        InstantiateItem();
    }
    private void Update()
    {
        if (limitMinPlayers)
        {
            if (PhotonNetwork.PlayerList.Length > 1)
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

    void InstantiateItem()
    {
        //Instantiates an object online.
        GameObject obj = playerList.AddObject();

        //Initialize it (for everyone)
        UIMultiplayerLobbyList script = obj.GetComponent<UIMultiplayerLobbyList>();
        script.Initialize(username, playerList.parent, false);

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
