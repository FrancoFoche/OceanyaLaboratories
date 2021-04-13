using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CharActions
{
    Attack,
    Defend,
    Skill,
    Item,
    Rearrange,
    Prepare,
    Skip
}

public class CharacterActions : MonoBehaviour
{
    public PlayerCharacter      caster;
    public List<PlayerCharacter> target = new List<PlayerCharacter>();
    public int maxTargets;
    public CharActions action;
    public string actionString;

    public BattleManager        manager;
    public CharacterUIList      uiList;
    public Button[]             actionButtons;

    [Header("Testing")]
    public int[]                skillsToActivate;

    private void Start()
    {
        actionButtons = GetComponentsInChildren<Button>();
    }

    private void Update()
    {
        if(manager.turnState == TurnState.WaitingForTarget)
        {
            if (Input.GetKeyDown(KeyCode.Return) || target.Count == maxTargets)
            {
                Act();
            }
            else
            {
                InteractableButtons(false);
                uiList.TurnToggleGroup(false);

                target = uiList.CheckTargets();
            }
        }
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
        if(TeamOrderManager.currentTurn != CharacterUIList.curCharacterSelected)
        {
            caster = CharacterUIList.curCharacterSelected;
        }
        else
        {
            caster = TeamOrderManager.currentTurn;
        }
    }
    /// <summary>
    /// Uses the action that was saved to actually run the code for the action.
    /// </summary>
    public void Act()
    {
        switch (action)
        {
            case CharActions.Attack:
                for (int i = 0; i < target.Count; i++)
                {
                    if (!target[i].dead)
                    {
                        if(target[i].targettable)
                        {
                            BattleManager.battleLog.LogBattleEffect($"{caster.name} attacks {target[i].name} for {caster.Attack(target[i])} DMG!");

                            if (target[i].stats[Stats.CURHP] <= 0)
                            {
                                target[i].dead = true;
                                BattleManager.battleLog.LogBattleEffect($"{target[i].name} is now dead as fuck!");
                            }
                        }
                        else
                        {
                            BattleManager.battleLog.LogBattleEffect($"{caster.name} attacks {target[i].name}... but {target[i].name} was untargettable. Attack misses.");
                        }
                        
                    }
                    else
                    {
                        
                        BattleManager.battleLog.LogBattleEffect($"{caster.name} attacks the dead body of {target[i].name}... How rude.");
                    }
                }

                BattleManager.UpdateUIs();
                manager.EndTurn();
                break;

            case CharActions.Defend:
                BattleManager.battleLog.LogBattleEffect($"{caster.name} defends!");
                BattleManager.battleLog.LogBattleEffect($"Yet for time reasons, the programmer couldn't make it work yet...");
                manager.EndTurn();
                break;

            case CharActions.Skill:
                BattleManager.battleLog.LogBattleEffect($"{caster.name} uses a Skill!");
                BattleManager.battleLog.LogBattleEffect($"Yet for time reasons, the programmer couldn't make it work yet...");
                manager.EndTurn();
                break;

            case CharActions.Item:
                BattleManager.battleLog.LogBattleEffect($"{caster.name} uses an Item!");
                BattleManager.battleLog.LogBattleEffect($"Yet for time reasons, the programmer couldn't make it work yet...");
                manager.EndTurn();
                break;

            case CharActions.Rearrange:
                BattleManager.battleLog.LogBattleEffect($"{caster.name} calls out a better Team Order!");
                BattleManager.battleLog.LogBattleEffect($"Yet for time reasons, the programmer couldn't make it work yet...");
                manager.EndTurn();
                break;

            case CharActions.Prepare:
                BattleManager.battleLog.LogBattleEffect($"{caster.name} prepares for an attack!");
                BattleManager.battleLog.LogBattleEffect($"Yet for time reasons, the programmer couldn't make it work yet...");
                manager.EndTurn();
                break;

            case CharActions.Skip:
                BattleManager.battleLog.LogBattleEffect($"{caster.name} skips their turn...");
                manager.EndTurn();
                break;
            default:
                print("default switch");
                break;
        }

        target = new List<PlayerCharacter>();
        InteractableButtons(true);
        uiList.TurnToggleGroup(true);
        uiList.SelectCharacter(TeamOrderManager.currentTurn);
        manager.turnState = TurnState.WaitingForAction;
    }

    public void Attack()
    {
        maxTargets = 1;
        ActionRequiresTarget(CharActions.Attack);
    }

    public void Defend()
    {
        ActionDoesNotRequireTarget(CharActions.Defend);
    }

    public void Skill()
    {
        ActionDoesNotRequireTarget(CharActions.Skill);
    }

    public void Item()
    {
        ActionDoesNotRequireTarget(CharActions.Item);
    }

    public void Rearrange()
    {
        ActionDoesNotRequireTarget(CharActions.Rearrange);
    }

    public void Prepare()
    {
        ActionDoesNotRequireTarget(CharActions.Prepare);
    }

    public void Skip()
    {
        ActionDoesNotRequireTarget(CharActions.Skip);
    }

    public void ActionRequiresTarget(CharActions action)
    {
        GetCaster();
        this.action = action;
        uiList.TurnToggles(false);
        manager.turnState = TurnState.WaitingForTarget;
    }

    public void ActionDoesNotRequireTarget(CharActions action)
    {
        GetCaster();
        this.action = action;
        Act();
    }
}
