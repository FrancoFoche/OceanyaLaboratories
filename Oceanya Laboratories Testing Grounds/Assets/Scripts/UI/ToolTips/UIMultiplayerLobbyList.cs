using System;
using Photon.Pun;
using Photon;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Photon.Realtime;

public class UIMultiplayerLobbyList : MonoBehaviourPunCallbacks, IPunObservable
{
    public struct Settings
    {
        public string username;
        public string nickname;
        public bool isMine;
        public PlayerCharacter character;
    }

    public TextMeshProUGUI username;
    public TMP_InputField nickname;
    [FormerlySerializedAs("character")] public UIPlayerCharacterDropdown playerCharacter;

    private void Start()
    {
        playerCharacter.SetCharacters(DBPlayerCharacter.pCharacters);
    }

    void SetUsername(string username)
    {
        this.username.text = username;
    }
    
    public Settings Confirm()
    {
        playerCharacter.Selected.SetNickname(nickname.text);
        playerCharacter.Selected.SetIsMine(photonView.IsMine);
        return new Settings() { username = username.text, nickname = nickname.text, character = playerCharacter.Selected, isMine = photonView.IsMine };
    }
    
    [PunRPC]
    private void SetInteractable(bool state)
    {
        nickname.interactable = state;
        playerCharacter.dropdown.interactable = state;
    }

    public void Initialize(string username, GameObject parent)
    {
        SetInteractable(true);
        photonView.RPC(nameof(SetInteractable), RpcTarget.Others, false);
        photonView.RPC(nameof(InitializeOnline), RpcTarget.All, username, parent.tag);
    }

    [PunRPC]
    void InitializeOnline(string username, string parentTag)
    {
        SetUsername(username);
        transform.parent = GameObject.FindGameObjectWithTag(parentTag).transform;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //Write to network

            stream.SendNext(username.text);
            stream.SendNext(playerCharacter.dropdown.value);
            stream.SendNext(nickname.text);
        }
        else
        {
            //Get from network

            SetUsername((string)stream.ReceiveNext());
            playerCharacter.dropdown.value = (int)stream.ReceiveNext();
            nickname.text = (string)stream.ReceiveNext();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Initialize(username.text, transform.parent.gameObject);
    }
}
