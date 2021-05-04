using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterActions : ButtonList
{
    public int maxTargets;
    public CharActions action;
    public Skill skillToActivate; //in case of using the skill action, this skill will activate after targetting
    public string actionString;

    public static UICharacterActions instance;

    private void Start()
    {
        instance = this;
    }

    public void AddAction(CharActions action)
    {
        GameObject newEntry = AddObject();
        newEntry.GetComponent<UIActionButton>().LoadAction(action);
        buttons.Add(newEntry.GetComponent<Button>());
    }
    public void AddAllActions()
    {
        buttons = new List<Button>();

        for (int i = 0; i < RuleManager.CharActionsHelper.Length; i++)
        {
            AddAction(RuleManager.CharActionsHelper[i]);
        }
    }

    /// <summary>
    /// Returns action description
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static string GetActionDescription(CharActions action)
    {
        string result = "Unknown Action";

        switch (action)
        {
            case CharActions.Attack:
                result = "Target an enemy, and if you win the SPD contest for hit, roll 20 for damage.";
                break;
            case CharActions.Defend:
                result = "For this turn, you take 50% damage from all sources except status effects.";
                break;
            case CharActions.Skill:
                result = "Use a skill from your skill list";
                break;
            case CharActions.Item:
                result = "Utilize an item from your inventory";
                break;
            case CharActions.Rearrange:
                result = "Use this turn to rearrange yourself in the team order. Both you and the one you swap with must agree to this arrangement.";
                break;
            case CharActions.Prepare:
                result = "Prepare yourself to be attacked, gain advantage in dodge roll IF you get attacked this turn. However, if a turn passes without being attacked while Ready, you gain DISTRACTED";
                break;
            case CharActions.Skip:
                result = "Just skips your turn without doing anything.";
                break;
        }

        return result;
    }

    /// <summary>
    /// Uses the action that was saved to actually run the code for the action.
    /// </summary>
    public void Act(Character caster, List<Character> target)
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
                                int basicAttackRaw = SkillFormula.ReadAndSumList(caster.basicAttackFormula, caster.stats);
                                int resultDMG = target[i].CalculateDefenses(basicAttackRaw, caster.basicAttackType);
                                BattleManager.battleLog.LogBattleEffect($"{caster.name} attacks {target[i].name} for {resultDMG} DMG!");
                                target[i].GetsDamagedBy(resultDMG);

                                if (!target[i].checkedPassives)
                                {
                                    target[i].checkedPassives = true;
                                    target[i].ActivatePassiveEffects(ActivationTime.WhenAttacked);
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

                    TeamOrderManager.EndTurn();
                }
                break;

            case CharActions.Defend:
                {
                    caster.SetDefending(true);
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
                    if (target[0].dead)
                    {
                        BattleManager.battleLog.LogBattleEffect($"{caster.name} tries to swap places with the dead body of {target[0].name}... but it doesn't work (choose another target)");
                        ActionRequiresTarget(CharActions.Rearrange);
                    }
                    else
                    {
                        TeamOrderManager.spdSystem.Swap(caster, target[0]);
                        BattleManager.battleLog.LogBattleEffect($"{caster.name} swaps places with {target[0].name} to delay their turn!");
                        TeamOrderManager.SetTurnState(TurnState.WaitingForAction);
                        TeamOrderManager.SetCurrentTurn(target[0]);
                        BattleManager.battleLog.LogBattleEffect($"{target[0].name}'s turn is skipped due to being swapped!");
                        TeamOrderManager.Continue();
                    }
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

        BattleManager.instance.ResetCheckedPassives();
        BattleManager.instance.ClearTargets();
    }

    /// <summary>
    /// The function a button uses to run its code.
    /// </summary>
    /// <param name="action"></param>
    public void ButtonAction(CharActions action)
    {
        Character caster = BattleManager.caster;

        switch (action)
        {
            case CharActions.Attack:
                maxTargets = 1;
                BattleManager.battleLog.LogBattleEffect($"{caster.name} attacks someone! (Choose a target)");
                ActionRequiresTarget(CharActions.Attack);
                break;

            case CharActions.Defend:
                ActionDoesNotRequireTarget(CharActions.Defend);
                break;

            case CharActions.Skill:
                if (caster.skillList.Count == 0)
                {
                    BattleManager.battleLog.LogBattleEffect($"{caster.name} has no skills to activate...");
                }
                else
                {
                    BattleManager.battleLog.LogBattleEffect($"{caster.name} uses a Skill!");
                    UISkillContext.instance.Show();
                }
                break;

            case CharActions.Item:
                ActionDoesNotRequireTarget(CharActions.Item);
                break;

            case CharActions.Rearrange:
                BattleManager.battleLog.LogBattleEffect($"{caster.name} chooses to swap with someone! (Choose a target)");
                maxTargets = 1;
                ActionRequiresTarget(CharActions.Rearrange);
                break;

            case CharActions.Prepare:
                ActionDoesNotRequireTarget(CharActions.Prepare);
                break;

            case CharActions.Skip:
                ActionDoesNotRequireTarget(CharActions.Skip);
                break;
        }
    }

    public void ActionRequiresTarget(CharActions action)
    {
        this.action = action;

        TeamOrderManager.SetTurnState(TurnState.WaitingForTarget);
    }
    public void ActionDoesNotRequireTarget(CharActions action)
    {
        this.action = action;
        Act(BattleManager.caster, BattleManager.target);
    }
}
