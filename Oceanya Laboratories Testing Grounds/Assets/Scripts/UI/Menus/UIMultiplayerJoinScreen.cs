using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIMultiplayerJoinScreen : MonoBehaviourPunCallbacks
{
    [Header("Assignables")]
    public TextMeshProUGUI joinNameInput;
    public TextMeshProUGUI createNameInput;
    public TextMeshProUGUI maxPlayersInput;
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI usernameInput;
    
    [Header("Connection")]
    public UnityEvent OnDisconnectedFromMasterAction;
    public UnityEvent OnConnectedToMasterAction;
    
    [Header("Join Room")]
    public UnityEvent OnJoinedRoomAction;
    public UnityEvent OnJoinedRoomErrorAction;

    [Header("Create Room")]
    public UnityEvent OnCreateRoomAction;
    public UnityEvent OnCreateRoomErrorAction;


    public void DisconnectFromServer()
    {
        PhotonNetwork.Disconnect();
    }
    public void ConnectToServer()
    {
        infoText.text = $"Connecting to master server...";
        PhotonNetwork.ConnectUsingSettings();
    }

    public void SetUsername()
    {
        MultiplayerLobbyManager.username = usernameInput.text;
    }
    
    public void CreateRoom()
    {
        RoomOptions options = new RoomOptions();

        options.MaxPlayers = 5;

        MultiplayerLobbyManager.lobbyName = createNameInput.text;
        PhotonNetwork.CreateRoom(createNameInput.text, options, TypedLobby.Default);
        
        infoText.text = $"Creating room {createNameInput.text}...";
    }
    
    public void JoinRoom()
    {
        MultiplayerLobbyManager.lobbyName = joinNameInput.text;
        PhotonNetwork.JoinRoom(joinNameInput.text);
        
        infoText.text = $"Joining room {joinNameInput.text}...";
    }
    
    #region Callbacks
    
    #region JoinRoom
    public override void OnJoinedRoom()
    {
        infoText.text = $"Joined!";
        
        OnJoinedRoomAction?.Invoke();
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        infoText.text = $"Error joining room: {message}";
        OnJoinedRoomErrorAction?.Invoke();
    }
    #endregion
    
    #region CreateRoom

    public override void OnCreatedRoom()
    {
        infoText.text = $"Created!";
        OnCreateRoomAction?.Invoke();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        infoText.text = $"Error creating room: {message}";
        OnCreateRoomErrorAction?.Invoke();
    }

    #endregion
    
    public override void OnConnectedToMaster()
    {
        OnConnectedToMasterAction?.Invoke();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        OnDisconnectedFromMasterAction?.Invoke();
    }

    #endregion
}
