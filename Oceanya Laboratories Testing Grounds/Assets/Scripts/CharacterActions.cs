using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterActions : MonoBehaviour
{
    public PlayerCharacter      caster;
    public PlayerCharacter      target { get { return DBPlayerCharacter.GetPC(1); } }
    public BattleManager        manager;

    public Button[]             actionButtons;

    [Header("Testing")]
    public int[]                skillsToActivate;

    private void Start()
    {
        actionButtons = GetComponentsInChildren<Button>();
    }

    public void InteractableButtons(bool state)
    {
        if(!state)
        {
            for (int i = 0; i < actionButtons.Length; i++)
            {
                actionButtons[i].interactable = false;
            }
        }
        else
        {
            for (int i = 0; i < actionButtons.Length; i++)
            {
                actionButtons[i].interactable = true;
            }
        }
    }

    public void GetCaster()
    {
        caster = manager.currentTurn;
    }

    public void Attack()
    {
        GetCaster();

        if(target.dead == false)
        {
            BattleManager.battleLog.LogBattleEffect($"{caster.name} attacks {target.name} for {caster.Attack(target)} DMG!");

            if(target.stats[Character.Stats.CURHP] <= 0)
            {
                target.dead = true;
                BattleManager.battleLog.LogBattleEffect($"{target.name} is now dead as fuck!");
            }
        }
        else
        {
            BattleManager.battleLog.LogBattleEffect($"{caster.name} attacks the dead body of {target.name}... How rude.");
        }

        BattleManager.UpdateUIs();

        manager.EndTurn();
    }

    public void Defend()
    {
        GetCaster();
        BattleManager.battleLog.LogBattleEffect($"{caster.name} defends!");
        BattleManager.battleLog.LogBattleEffect($"Yet for time reasons, the programmer couldn't make it work yet...");
        manager.EndTurn();
    }

    public void Skill()
    {
        GetCaster();
        Testing();
        manager.EndTurn();
    }

    public void Item()
    {
        GetCaster();
        BattleManager.battleLog.LogBattleEffect($"{caster.name} uses an Item!");
        BattleManager.battleLog.LogBattleEffect($"Yet for time reasons, the programmer couldn't make it work yet...");
        manager.EndTurn();
    }

    public void Rearrange()
    {
        GetCaster();
        BattleManager.battleLog.LogBattleEffect($"{caster.name} calls out a better Team Order!");
        BattleManager.battleLog.LogBattleEffect($"Yet for time reasons, the programmer couldn't make it work yet...");
        manager.EndTurn();
    }

    public void Prepare()
    {
        GetCaster();
        BattleManager.battleLog.LogBattleEffect($"{caster.name} prepares for an attack!");
        BattleManager.battleLog.LogBattleEffect($"Yet for time reasons, the programmer couldn't make it work yet...");
        manager.EndTurn();
    }

    public void Skip()
    {
        GetCaster();
        BattleManager.battleLog.LogBattleEffect($"{caster.name} skips their turn...");
        manager.EndTurn();
    }

    void Testing()
    {
        for (int i = 0; i < skillsToActivate.Length; i++)
        {
            caster.activeSkillList[skillsToActivate[i]].Activate(caster, target);
        }
    }
}
