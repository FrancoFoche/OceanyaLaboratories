using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MultiplayerBattleSynchronization : MonoBehaviourPunCallbacks, IPunObservable
{
    private List<BattleUI> _enemyUIs;
    private BattleUI _ui;
    private Character character;
    private PhotonAnimatorView _animatorView;
    public BattleUI ui
    {
        get => _ui;
        set
        {
            _ui = value;
        }
    }

    public void Initialize(int id)
    {
        InitializeOnline(id);
    }

    [PunRPC]
    void InitializeLocal(int id)
    {
        ui = BattleManager.i.uiList.GetUIByAllyID(id);
        character = ui.loadedChar;
        _animatorView = ui.gameObject.GetComponentInChildren<PhotonAnimatorView>();
        
        _enemyUIs = BattleManager.i.uiList.GetEnemyUIs();
    }

    void InitializeOnline(int id)
    {
        photonView.RPC(nameof(InitializeLocal), RpcTarget.All, id);
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        ui.OnPhotonSerializeView(stream, info);
        
        foreach (var enemy in _enemyUIs)
        {
            enemy.OnPhotonSerializeView(stream, info);
        }
        
        _animatorView.OnPhotonSerializeView(stream, info);
    }


}
