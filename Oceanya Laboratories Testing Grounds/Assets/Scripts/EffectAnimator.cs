using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EffectAnimator : MonoBehaviourPun
{
    public enum Effects
    {
        Attack,
        Debuff,
        Buff,
        Heal,
        Death,
        Revive,
        Explosion,
        Special
    }

    public BattleUI ui;
    public Animator animator;

    #region Animator Event Actions
    public void PlayEffect(Effects effect)
    {
        if (MultiplayerBattleManager.multiplayerActive)
        {
            PlayEffectLocal(effect);
            //photonView.RPC(nameof(PlayEffectLocal), RpcTarget.All, effect);
        }
        else
        {
            PlayEffectLocal(effect);
        }
    }
    [PunRPC]
    public void PlayEffectLocal(Effects effect)
    {
        Debug.Log("Played " + effect.ToString() + " effect.");
        animator.SetTrigger(effect.ToString());
    }
    
    public void PlaySound(Sounds sound)
    {
        if (MultiplayerBattleManager.multiplayerActive)
        {
            PlaySoundLocal(sound);
            //photonView.RPC(nameof(PlaySoundLocal), RpcTarget.All, sound);
        }
        else
        {
            PlaySoundLocal(sound);
        }
    }
    
    public void PlaySoundLocal(Sounds sound)
    {
        SFXManager.PlaySound(sound);
    }
    public void CheckTeamDeath()
    {
        Debug.Log("Total team death: Act");
        
        BattleManager.i.TotalTeamKill_Act();
    }
    public void UpdateUI()
    {
        ui.UpdateUI();
    }
    #endregion
}
