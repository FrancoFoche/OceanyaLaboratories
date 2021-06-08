using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAnimator : MonoBehaviour
{
    public enum Effects
    {
        Attack,
        Debuff,
        Buff,
        Heal,
        Death,
        Revive
    }

    public BattleUI ui;
    public Animator animator;

    private void Update()
    {
        Testing();
    }

    #region Animator Event Actions
    public void PlayEffect(Effects effect)
    {
        Debug.Log("Played " + effect.ToString() + " effect.");
        animator.SetTrigger(effect.ToString());
    }
    public void PlaySound(Sounds sound)
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


    /// <summary>
    /// Just a testing function that reads your input
    /// </summary>
    public void Testing()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayEffect(Effects.Attack);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayEffect(Effects.Debuff);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlayEffect(Effects.Buff);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PlayEffect(Effects.Heal);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            PlayEffect(Effects.Death);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            PlayEffect(Effects.Revive);
        }
    }
}
