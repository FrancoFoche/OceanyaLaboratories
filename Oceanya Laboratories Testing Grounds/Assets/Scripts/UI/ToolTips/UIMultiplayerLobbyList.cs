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
        public int id;
        public bool isMine;
        public PlayerCharacter character;
    }

    public TextMeshProUGUI username;
    public TMP_InputField nickname;
    [FormerlySerializedAs("character")] public UIPlayerCharacterDropdown playerCharacter;
    public int id;

    private bool wasGivenAnId = false;
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
        return new Settings()
        {
            username = username.text,
            nickname = nickname.text,
            character = playerCharacter.Selected,
            isMine = photonView.IsMine,
            id = id
        };
    }
    
    

    public void Initialize(string username, GameObject parent, bool wasGivenAnId)
    {
        SetInteractable(true);
        if (!wasGivenAnId)
        {
            id = PhotonNetwork.PlayerList.Length;
            this.wasGivenAnId = true;
        }
        
        photonView.RPC(nameof(SetInteractable), RpcTarget.Others, false);
        photonView.RPC(nameof(InitializeOnline), RpcTarget.All, username, parent.tag);
        photonView.RPC(nameof(AddToListOnline), RpcTarget.Others, parent.tag);
    }

    [PunRPC]
    private void SetInteractable(bool state)
    {
        nickname.interactable = state;
        playerCharacter.dropdown.interactable = state;
    }
    
    [PunRPC]
    void InitializeOnline(string username, string parentTag)
    {
        SetUsername(username);
        transform.SetParent(GameObject.FindGameObjectWithTag(parentTag).transform);
    }
    
    [PunRPC]
    void AddToListOnline(string parentTag)
    {
        GameObject.FindGameObjectWithTag(parentTag)
            .GetComponentInParent<ObjectList>()
            .list.Add(gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //Write to network

            stream.SendNext(username.text);
            stream.SendNext(playerCharacter.dropdown.value);
            stream.SendNext(nickname.text);
            stream.SendNext(id);
        }
        else
        {
            //Get from network

            SetUsername((string)stream.ReceiveNext());
            playerCharacter.dropdown.value = (int)stream.ReceiveNext();
            nickname.text = (string)stream.ReceiveNext();
            id = (int)stream.ReceiveNext();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Initialize(username.text, transform.parent.gameObject, wasGivenAnId);
    }
}
