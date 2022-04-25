using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;

public class MultiplayerBattleManager : MonoBehaviourPunCallbacks
{
    public string pathPrefix;
    public GameObject syncHelper;
    public static bool multiplayerActive = false;
    public static UIMultiplayerLobbyList.Settings[] players;
    public static MultiplayerBattleSynchronization playerSync;

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
            //Instantiate the player sync helper
            InstantiateItem();
        }
    }

    public void AttachToPlayer()
    {
        playerSync?.Initialize(players.First(x => x.isMine).character.ID);
    }

    void InstantiateItem()
    {
        //Instantiates an object online.
        GameObject obj = PhotonNetwork.Instantiate(pathPrefix+syncHelper.name, transform.position, transform.rotation);

        //Get all the info necessary
        UIMultiplayerLobbyList.Settings player = players.First(x => x.isMine);
        
        
        //Initialize it (for everyone)
        MultiplayerBattleSynchronization script = obj.GetComponent<MultiplayerBattleSynchronization>();
        script.Initialize(player.character.ID);

        playerSync = script;
    }

    #region CALLBACKS

    public override void OnDisconnected(DisconnectCause cause)
    {
        playerSync = null;
        SceneLoaderManager.instance.LoadMainMenu();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PhotonNetwork.Disconnect();
    }

    #endregion
}
