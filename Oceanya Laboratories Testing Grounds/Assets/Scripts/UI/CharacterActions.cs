using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterActions : MonoBehaviour
{
    public static Character caster;
    public static List<Character> target = new List<Character>();
    public int maxTargets;
    public CharActions action;
    public Skill skillToActivate; //in case of using the skill action, this skill will activate after targetting
    public string actionString;

    public static CharacterActions instance;

    public Button[] actionButtons;



    private void Start()
    {
        actionButtons = GetComponentsInChildren<Button>();
        instance = this;
    }

    private void Update()
    {
        if (BattleManager.instance.turnState == TurnState.WaitingForTarget)
        {
            if (Input.GetKeyDown(KeyCode.Return) || target.Count == maxTargets)
            {
                Act();
            }
            else
            {
                InteractableButtons(false);
                BattleManager.charUIList.TurnToggleGroup(false);

                target = BattleManager.charUIList.CheckTargets();
            }
        }
        else
        {
            if (BattleManager.charUIList.toggleGroup.AnyTogglesOn() && BattleManager.charUIList.different)
            {
                GetCaster();
                UISkillContext.instance.Hide();
            }
        }
    }

    public void InteractableButtons(bool state)
    {
        if (!state)
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
        if (TeamOrderManager.currentTurn != AllyUIList.curCharacterSelected)
        {
            caster = AllyUIList.curCharacterSelected;
            BattleManager.battleLog.LogBattleStatus($"{caster.name}'s (Non-Ordered) Turn ");
        }
        else
        {
            caster = TeamOrderManager.currentTurn;
        }

        UISkillContext.instance.LoadSkills(caster);
    }
    /// <summary>
    /// Uses the action that was saved to actually run the code for the action.
    /// </summary>
    public void Act()
    {
        switch (action)
        {
            case CharActions.Attack:
                {
                    for (int i = 0; i < target.Count; i++)
                    {
                        if (!target[i].dead)
                        {
                            if (target[i].targettable)
                            {
                                BattleManager.battleLog.LogBattleEffect($"{caster.name} attacks {target[i].name} for {caster.Attack(target[i])} DMG!");

                                if (!target[i].checkedPassives)
                                {
                                    target[i].checkedPassives = true;
                                    target[i].ActivatePassiveEffects(PassiveActivation.WhenAttacked);
                                }

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
                    TeamOrderManager.EndTurn();
                }
                break;

            case CharActions.Defend:
                {
                    BattleManager.battleLog.LogBattleEffect($"{caster.name} defends!");
                    BattleManager.battleLog.LogBattleEffect($"Yet for time reasons, the programmer couldn't make it work yet...");
                    TeamOrderManager.EndTurn();
                }
                break;

            case CharActions.Skill:
                {
                    skillToActivate.SkillAction(caster, target);
                }
                break;

            case CharActions.Item:
                {
                    BattleManager.battleLog.LogBattleEffect($"{caster.name} uses an Item!");
                    BattleManager.battleLog.LogBattleEffect($"Yet for time reasons, the programmer couldn't make it work yet...");
                    TeamOrderManager.EndTurn();
                }
                break;

            case CharActions.Rearrange:
                {
                    BattleManager.battleLog.LogBattleEffect($"{caster.name} calls out a better Team Order!");
                    BattleManager.battleLog.LogBattleEffect($"Yet for time reasons, the programmer couldn't make it work yet...");
                    TeamOrderManager.EndTurn();
                }
                break;

            case CharActions.Prepare:
                {
                    BattleManager.battleLog.LogBattleEffect($"{caster.name} prepares for an attack!");
                    BattleManager.battleLog.LogBattleEffect($"Yet for time reasons, the programmer couldn't make it work yet...");
                    TeamOrderManager.EndTurn();
                }
                break;

            case CharActions.Skip:
                {
                    BattleManager.battleLog.LogBattleEffect($"{caster.name} skips their turn...");
                    TeamOrderManager.EndTurn();
                }
                break;
            default:
                print("default switch");
                break;
        }

        ResetCheckedPassives();
        target = new List<Character>();
        InteractableButtons(true);
        BattleManager.charUIList.TurnToggleGroup(true);
        BattleManager.charUIList.SelectCharacter(TeamOrderManager.currentTurn);
        BattleManager.instance.turnState = TurnState.WaitingForAction;
    }

    public void Attack()
    {
        maxTargets = 1;
        BattleManager.battleLog.LogBattleEffect($"{caster.name} attacks someone! (Choose a target)");
        ActionRequiresTarget(CharActions.Attack);
    }
    public void Defend()
    {
        ActionDoesNotRequireTarget(CharActions.Defend);
    }
    public void Skill()
    {
        if (caster.skillList.Count == 0)
        {
            BattleManager.battleLog.LogBattleEffect($"{caster.name} has no skills to activate...");
        }
        else
        {
            BattleManager.battleLog.LogBattleEffect($"{caster.name} uses a Skill!");
            UISkillContext.instance.Show();
        }
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
        this.action = action;
        BattleManager.charUIList.TurnToggles(false);
        BattleManager.instance.turnState = TurnState.WaitingForTarget;
    }
    public void ActionDoesNotRequireTarget(CharActions action)
    {
        this.action = action;
        Act();
    }
    public void ResetCheckedPassives()
    {
        caster.checkedPassives = false;

        for (int i = 0; i < target.Count; i++)
        {
            target[i].checkedPassives = false;
        }
    }
}
