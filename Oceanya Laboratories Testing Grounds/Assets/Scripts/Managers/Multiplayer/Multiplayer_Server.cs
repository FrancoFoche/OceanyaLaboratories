using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;

public class Multiplayer_Server : MonoBehaviourPunCallbacks
{
    public string pathPrefix;
    public GameObject syncHelper;
    public static bool multiplayerActive = false;
    public static UIMultiplayerLobbyList.Settings[] players;
    
    public static Multiplayer_Client localPlayer;
    
    public static Player serverHost;
    public static Dictionary<Player, Multiplayer_Client> clients = new Dictionary<Player, Multiplayer_Client>();
    
    public static List<Character> GetAllyCharacters()
    {
        //List<Character> unorderedList = players.Select(x => (Character)x.character).ToList();
        List<Character> orderedList = new List<Character>();

        for (int id = 1; id < players.Length+1; id++)
        {
            orderedList.Add(Array.Find(players,x => x.id == id).character);
        }

        return orderedList;
    }

    public void OnStart()
    {
        if (multiplayerActive)
        {
            //Instantiate the player client
            InstantiateClientObject();
        }
    }
    
    public void AttachToPlayer()
    {
        localPlayer?.Initialize(players.First(x => x.isMine).character.ID);
    }

    void InstantiateClientObject()
    {
        //Instantiates an object online.
        GameObject obj = PhotonNetwork.Instantiate(pathPrefix+syncHelper.name, transform.position, transform.rotation);

        //Get all the info necessary
        UIMultiplayerLobbyList.Settings player = players.First(x => x.isMine);
        
        
        //Initialize it (for everyone)
        Multiplayer_Client client = obj.GetComponent<Multiplayer_Client>();
        client.Initialize(player.character.ID);

        localPlayer = client;
    }

    #region Server Requests
    public void RequestAct(Player player, ActionData data)
    {
        photonView.RPC(nameof(RPC_Act), serverHost, player, data);
    }
    #endregion

    #region Local Execution
    [PunRPC]
    private void RPC_Act(Player playerRequested, ActionData data)
    {
        if (clients.ContainsKey(playerRequested))
        {
            UICharacterActions.instance.Act(data);
        }
    }
    #endregion
    
    #region CALLBACKS
    public override void OnDisconnected(DisconnectCause cause)
    {
        localPlayer = null;
        SceneLoaderManager.instance.LoadMainMenu();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PhotonNetwork.Disconnect();
    }

    #endregion
}
