using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kam.TooltipUI;

public enum BattleState
{
    Start,
    InCombat,
    Won,
    Lost,
    End
}
public class Wave
{
    public List<Character> allySide;
    public List<Character> enemySide;
    public Music waveMusic = Music.None;
    public bool winMusicPlay = true;
    public bool lossMusicPlay = true;
    public int transitionOutTime = 5;
    public int EXPgiven = 0;
    public int GoldGiven = 0;
}

public class BattleManager : MonoBehaviour
{
    private static BattleManager _instance;
    public static BattleManager i { get { if (_instance == null) { _instance = FindObjectOfType<BattleManager>(); } return _instance; } private set { _instance = value; } }


    public static   Character           caster              { get; private set; }
    public static   List<Character>     target              { get; private set; }

    public static   LevelManager.BattleLevel currentLevel   { get; private set; }
    public static   int                 currentBattleIndex  { get; private set; }

    public          BattleState         battleState         { get; private set; }

    public          bool                inCombat            { get; private set; }
    public          bool                enemyTeamDeath      { get; private set; }
    public          bool                allyTeamDeath       { get; private set; }

    public          BattleLog                   battleLog;
    public          BattleUIList                uiList;
    public          UICharacterActions          charActions;
    public          PauseMenu                   pauseMenu;
    public          TooltipPopup                tooltipPopup;
    public          UIMenu_TeamOrder            teamOrderMenu;

    public          GameObject                  easteregg;

    float exitTime = 1.5f;
    float curHold;

    private void Awake()
    {
        DataBaseOrder.i.Initialize();
    }

    private void Start()
    {
        #region One time only Initializations
        currentBattleIndex = 0;

        inCombat = false;

        SaveFile loaded = SavesManager.loadedFile;
        if (loaded != null)
        {
            teamOrderMenu.dropdownToggle.isOn = loaded.showOrderOfPlay;
            teamOrderMenu.showDead.isOn = loaded.orderOfPlay_showDead;
            teamOrderMenu.showPast.isOn = loaded.orderOfPlay_showPast;
        }

        pauseMenu.manualMode.isOn = SettingsManager.manualMode;
        pauseMenu.confirmActions.isOn = SettingsManager.actionConfirmation;
        #endregion

        currentLevel = LevelManager.GetLevel(LevelManager.currentLevel);

        StartCombat(currentLevel.waves[0]);
    }

    private void Update()
    {
        if (!battleLog.chatBox.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (TeamOrderManager.turnState == TurnState.WaitingForConfirmation || TeamOrderManager.turnState == TurnState.WaitingForTarget)
                {
                    if (UIActionConfirmationPopUp.i.waitingForConfirmation)
                    {
                        UIActionConfirmationPopUp.i.Deny();
                    }

                    if (UICharacterActions.instance.waitingForConfirmation)
                    {
                        UICharacterActions.instance.DenyAction();
                    }
                }
                else
                {
                    pauseMenu.TogglePause();
                }
            }

            if (Input.GetKey(KeyCode.Escape))
            {
                curHold += Time.deltaTime;

                if (curHold > exitTime)
                {
                    SceneLoaderManager.instance.Quit();
                }
            }

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                curHold = 0;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (UIActionConfirmationPopUp.i.waitingForConfirmation)
                {
                    UIActionConfirmationPopUp.i.Confirm();
                }
                else
                {
                    if (!TeamOrderManager.AIturn)
                    {
                        switch (TeamOrderManager.turnState)
                        {
                            case TurnState.WaitingForAction:
                            case TurnState.WaitingForConfirmation:
                                UICharacterActions.instance.ButtonAction(CharActions.EndTurn);
                                break;

                            case TurnState.WaitingForTarget:
                                Action temp = delegate
                                {
                                    Debug.Log("Targetting done.");
                                    TeamOrderManager.SetTurnState(TurnState.WaitingForConfirmation);
                                };

                                if (target.Count == 0)
                                {
                                    UIActionConfirmationPopUp.i.Show(temp, true, "You have selected no targets, are you sure you want to confirm your action?");
                                }
                                else
                                {
                                    temp();
                                }
                                break;
                        }
                    }
                }
            }

            if (!pauseMenu.paused)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    teamOrderMenu.ToggleVisibility();
                }

                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        UIActionConfirmationPopUp.i.Show(delegate { SavesManager.DeleteSave(); SceneLoaderManager.instance.ReloadScene(); }, false, "Are you ABSOLUTELY sure that you want to delete your save file?");
                    }
                    else
                    {
                        UIActionConfirmationPopUp.i.Show(() => SceneLoaderManager.instance.ReloadScene(), false, "Are you sure you want to restart the battle?");
                    }
                }

                if (TeamOrderManager.turnState != TurnState.End && TeamOrderManager.turnState != TurnState.Start && TeamOrderManager.turnState != TurnState.NonDefined)
                {
                    if (TeamOrderManager.turnState == TurnState.WaitingForTarget || TeamOrderManager.turnState == TurnState.WaitingForConfirmation)
                    {
                        if (TeamOrderManager.turnState == TurnState.WaitingForTarget)
                        {
                            if (target.Count == UICharacterActions.instance.maxTargets)
                            {
                                Debug.Log("Targetting done.");
                                TeamOrderManager.SetTurnState(TurnState.WaitingForConfirmation);
                            }
                            else
                            {
                                if (caster.team == Team.Ally || SettingsManager.manualMode)
                                {
                                    SetTargets(uiList.CheckTargets());
                                }
                            }
                        }

                        if (!TeamOrderManager.AIturn)
                        {
                            if (Input.GetMouseButtonDown(1))
                            {
                                UICharacterActions.instance.DenyAction();
                            }
                        }
                    }
                    else if (TeamOrderManager.turnState == TurnState.WaitingForAction)
                    {
                        if (!UIActionConfirmationPopUp.i.waitingForConfirmation)
                        {
                            if (Input.GetKeyDown(KeyCode.Q))
                            {
                                UICharacterActions.instance.ButtonAction(CharActions.Attack);
                            }

                            if (Input.GetKeyDown(KeyCode.W))
                            {
                                UICharacterActions.instance.ButtonAction(CharActions.Defend);
                            }

                            if (Input.GetKeyDown(KeyCode.E))
                            {
                                UICharacterActions.instance.ButtonAction(CharActions.Skill);
                            }

                            if (Input.GetKeyDown(KeyCode.R))
                            {
                                UICharacterActions.instance.ButtonAction(CharActions.Item);
                            }

                            if (Input.GetKeyDown(KeyCode.T))
                            {
                                UICharacterActions.instance.ButtonAction(CharActions.Rearrange);
                            }

                            if (Input.GetKeyDown(KeyCode.Y))
                            {
                                UICharacterActions.instance.ButtonAction(CharActions.Prepare);
                            }

                            if (UISkillContext.instance.gameObject.activeSelf)
                            {
                                if (Input.GetKeyDown(KeyCode.Alpha1))
                                {
                                    UISkillContext.instance.ActivateButtonInPosition(1);
                                }

                                if (Input.GetKeyDown(KeyCode.Alpha2))
                                {
                                    UISkillContext.instance.ActivateButtonInPosition(2);
                                }

                                if (Input.GetKeyDown(KeyCode.Alpha3))
                                {
                                    UISkillContext.instance.ActivateButtonInPosition(3);
                                }

                                if (Input.GetKeyDown(KeyCode.Alpha4))
                                {
                                    UISkillContext.instance.ActivateButtonInPosition(4);
                                }

                                if (Input.GetKeyDown(KeyCode.Alpha5))
                                {
                                    UISkillContext.instance.ActivateButtonInPosition(5);
                                }

                                if (Input.GetKeyDown(KeyCode.Alpha6))
                                {
                                    UISkillContext.instance.ActivateButtonInPosition(6);
                                }

                                if (Input.GetKeyDown(KeyCode.Alpha7))
                                {
                                    UISkillContext.instance.ActivateButtonInPosition(7);
                                }

                                if (Input.GetKeyDown(KeyCode.Alpha8))
                                {
                                    UISkillContext.instance.ActivateButtonInPosition(8);
                                }

                                if (Input.GetKeyDown(KeyCode.Alpha9))
                                {
                                    UISkillContext.instance.ActivateButtonInPosition(9);
                                }

                                if (Input.GetKeyDown(KeyCode.Alpha0))
                                {
                                    UISkillContext.instance.ActivateButtonInPosition(10);
                                }
                            }

                            if (UIItemContext.instance.gameObject.activeSelf)
                            {
                                if (Input.GetKeyDown(KeyCode.Alpha1))
                                {
                                    UIItemContext.instance.ActivateButtonInPosition(1);
                                }

                                if (Input.GetKeyDown(KeyCode.Alpha2))
                                {
                                    UIItemContext.instance.ActivateButtonInPosition(2);
                                }

                                if (Input.GetKeyDown(KeyCode.Alpha3))
                                {
                                    UIItemContext.instance.ActivateButtonInPosition(3);
                                }

                                if (Input.GetKeyDown(KeyCode.Alpha4))
                                {
                                    UIItemContext.instance.ActivateButtonInPosition(4);
                                }

                                if (Input.GetKeyDown(KeyCode.Alpha5))
                                {
                                    UIItemContext.instance.ActivateButtonInPosition(5);
                                }

                                if (Input.GetKeyDown(KeyCode.Alpha6))
                                {
                                    UIItemContext.instance.ActivateButtonInPosition(6);
                                }

                                if (Input.GetKeyDown(KeyCode.Alpha7))
                                {
                                    UIItemContext.instance.ActivateButtonInPosition(7);
                                }

                                if (Input.GetKeyDown(KeyCode.Alpha8))
                                {
                                    UIItemContext.instance.ActivateButtonInPosition(8);
                                }

                                if (Input.GetKeyDown(KeyCode.Alpha9))
                                {
                                    UIItemContext.instance.ActivateButtonInPosition(9);
                                }

                                if (Input.GetKeyDown(KeyCode.Alpha0))
                                {
                                    UIItemContext.instance.ActivateButtonInPosition(10);
                                }
                            }
                        }

                        if (SettingsManager.manualMode)
                        {
                            uiList.CheckCurrentSelection();
                            bool togglesOn = uiList.toggleGroup.AnyTogglesOn();
                            if (togglesOn && uiList.different)
                            {
                                uiList.UpdateSelected();
                                GetCaster();
                            }

                            #region Debug Features
                            if (Input.GetKeyDown(KeyCode.LeftControl) && caster != TeamOrderManager.currentTurn)
                            {
                                battleLog.LogBattleEffect("The GM decided to revert back to the turn that was supposed to take place. Smh.");
                                ReselectOriginalTurn();
                            }
                            #endregion
                        }
                        else
                        {
                            if (BattleUIList.curCharacterSelected != TeamOrderManager.currentTurn && battleState == BattleState.InCombat)
                            {
                                Debug.LogWarning("The half-assed bugfix patch was triggered.");
                                uiList.SelectCharacter(TeamOrderManager.currentTurn);
                            }
                        }
                    }
                }
            }
        }
    }
    public void                     SetBattleIndexToMax ()                          
    {
        currentBattleIndex = currentLevel.waves.Length;
    }
    public void                     StartCombat         (Wave combat)               
    {
        easteregg.gameObject.SetActive(false);

        enemyTeamDeath = false;
        allyTeamDeath = false;
        
        if(combat.waveMusic == Music.None)
        {
            MusicManager.PlayMusic(currentLevel.levelMusic);
        }
        else
        {
            MusicManager.PlayMusic(combat.waveMusic);
        }
        
        TeamOrderManager.BuildTeamOrder(combat);
        SetBattleState(BattleState.Start);
    }
    public void                     SetupBattle         ()                          
    {
        int index = 0;
        while (TeamOrderManager.spdSystem.teamOrder[index].dead)
        {
            index++;
        }
        caster = TeamOrderManager.spdSystem.teamOrder[index];
        TeamOrderManager.SetCurrentTurn(caster);
        TeamOrderManager.SetTurnState(TurnState.Start);
        SetBattleState(BattleState.InCombat);
    }           
    
    public void                     SetBattleState      (BattleState state)         
    {
        if (battleState == state && battleState != BattleState.Start)
        {
            Debug.Log("Battle state is ALREADY set to " + state.ToString() + ".");
        }
        else
        {
            SpriteRenderer[] array = easteregg.GetComponentsInChildren<SpriteRenderer>();
            Wave currentWave = currentLevel.waves[currentBattleIndex];

            switch (state)
            {
                case BattleState.Start:
                    {
                        battleState = BattleState.Start;

                        caster = new Character();
                        target = new List<Character>();

                        charActions.AddAllActions();
                        uiList.SetSides(TeamOrderManager.allySide, TeamOrderManager.enemySide);
                        

                        for (int i = 0; i < TeamOrderManager.totalCharList.Count; i++)
                        {
                            TeamOrderManager.totalCharList[i].CheckPassives();
                            TeamOrderManager.totalCharList[i].ActivatePassiveEffects(ActivationTime_General.StartOfBattle);
                        }
                        charActions.InteractableButtons(false);
                        uiList.InteractableUIs(false);
                        battleLog.LogBattleStatus("COMBAT START!");

                        DelayAction(3, SetupBattle);
                    }
                    break;

                case BattleState.InCombat:
                    {
                        inCombat = true;
                        battleState = BattleState.InCombat;
                    }
                    break;

                case BattleState.Won:
                    {
                        battleState = BattleState.Won;
                        if (currentLevel.waves[currentBattleIndex].winMusicPlay)
                        {
                            MusicManager.PlayMusic(Music.Win);
                        }
                        
                        charActions.InteractableButtons(false);
                        uiList.InteractableUIs(false);
                        battleLog.LogBattleStatus("Ally team wins!");
                        easteregg.SetActive(true);
                        for (int i = 0; i < array.Length; i++)
                        {
                            array[i].color = Color.green;
                        }
                        SetBattleState(BattleState.End);
                    }
                    break;

                case BattleState.Lost:
                    {
                        battleState = BattleState.Lost;
                        if (currentLevel.waves[currentBattleIndex].lossMusicPlay)
                        {
                            MusicManager.PlayMusic(Music.Lose);
                        }
                        
                        charActions.InteractableButtons(false);
                        uiList.InteractableUIs(false);
                        battleLog.LogBattleStatus("Enemy team wins!");
                        easteregg.SetActive(true);
                        for (int i = 0; i < array.Length; i++)
                        {
                            array[i].color = Color.red;
                        }
                        SetBattleState(BattleState.End);
                    }
                    break;

                case BattleState.End:
                    {
                        UISkillContext.instance.Hide();
                        UIItemContext.instance.Hide();

                        if (battleState == BattleState.Lost)
                        {
                            currentBattleIndex = 0;
                            battleLog.LogCountdown(5, "Restarting battle in _countdown_...", () => StartCombat(currentLevel.waves[0]));
                            return;
                        }

                        battleState = BattleState.End;

                        GiveAllyTeamEXP(currentWave.EXPgiven);

                        SettingsManager.groupGold += currentWave.GoldGiven;
                        battleLog.LogImportant("TEAM GAINS " + currentWave.GoldGiven + $"G! Current: {SettingsManager.groupGold}G");

                        DelayAction(3, delegate
                        {
                            currentBattleIndex++;
                            if (currentBattleIndex < currentLevel.waves.Length)
                            {
                                battleLog.LogCountdown(currentWave.transitionOutTime, "Next wave starts in _countdown_...", () => StartCombat(currentLevel.waves[currentBattleIndex]));
                            }
                            else
                            {
                                battleLog.LogCountdown(currentLevel.waves[currentLevel.waves.Length-1].transitionOutTime, $"Going to {currentLevel.winningScene.ToString()} in _countdown_...", () => SceneLoaderManager.instance.LoadScene(currentLevel.winningScene));
                            }
                        });

                        if (SettingsManager.lastClearedLevel < LevelManager.currentLevel)
                        {
                            SettingsManager.lastClearedLevel = LevelManager.currentLevel;
                        }

                        SettingsManager.SaveSettings();
                    }
                    break;
            }
        }
    }
    public void                     TotalTeamKill_Check ()                          
    {
        if (battleState != BattleState.End)
        {
            int AllyCount = TeamOrderManager.allySide.Count;
            int AllyDeathCount = 0;

            for (int i = 0; i < AllyCount; i++)
            {
                if (TeamOrderManager.allySide[i].dead)
                {
                    AllyDeathCount++;
                }
            }

            if (AllyDeathCount == AllyCount)
            {
                allyTeamDeath = true;
                inCombat = false;
                return;
            }


            int EnemyCount = TeamOrderManager.enemySide.Count;
            int EnemyDeathCount = 0;

            for (int i = 0; i < EnemyCount; i++)
            {
                if (TeamOrderManager.enemySide[i].dead)
                {
                    EnemyDeathCount++;
                }
            }

            if (EnemyDeathCount == EnemyCount)
            {
                enemyTeamDeath = true;
                inCombat = false;
                return;
            }
        }
    }
    public void                     TotalTeamKill_Act   ()                          
    {
        if (battleState != BattleState.End)
        {
            if (allyTeamDeath)
            {
                SetBattleState(BattleState.Lost);
                return;
            }

            if (enemyTeamDeath)
            {
                SetBattleState(BattleState.Won);
                return;
            }
        }
    }
    public void                     GetCaster           ()                          
    {
        if (TeamOrderManager.currentTurn != BattleUIList.curCharacterSelected && !TeamOrderManager.AIturn)
        {
            caster = BattleUIList.curCharacterSelected;
            battleLog.LogTurn(caster, 3);
            TeamOrderManager.SetTurnState(TurnState.Start);
        }
        else
        {
            caster = TeamOrderManager.currentTurn;
        }
    }
    public void                     SetTargets          (List<Character> targets)   
    {
        if(target.Count != targets.Count)
        {
            Debug.Log("Set targets, current count: " + targets.Count);
        }
        
        target = targets;
        
    }
    public void                     ClearTargets        ()                          
    {
        target = new List<Character>();
        Debug.Log("Cleared targets");
    }
    public void                     ResetCheckedPassives()                          
    {
        caster.SetCheckedPassives(false);

        for (int i = 0; i < target.Count; i++)
        {
            target[i].SetCheckedPassives(false);
        }
    }
    public void                     ReselectOriginalTurn()                          
    {
        uiList.SelectCharacter(TeamOrderManager.currentTurn);
        battleLog.LogTurn(TeamOrderManager.currentTurn, 2);
    }
    public void                     GiveAllyTeamEXP     (int exp)                   
    {
        List<Character> characters = uiList.GetTeam(Team.Ally);

        for (int i = 0; i < characters.Count; i++)
        {
            if (!characters[i].dead)
            {
                characters[i].AddExp(exp);
            }
            else
            {
                battleLog.LogBattleEffect(characters[i].name + " was dead and could not receive EXP.");
            }
        }
    }
    public void                     UpdateTeamOrder     ()                          
    {
        TeamOrderManager.BuildSPDSystem(TeamOrderManager.allySide, TeamOrderManager.enemySide);
    }


    #region Utilities
    public void                     DelayAction         (float secondsToDelay, Action delayedAction)      
    {
        StartCoroutine(Delay(secondsToDelay, delayedAction));
    }

    IEnumerator                     Delay               (float secondsToDelay, Action delayedAction)
    {
        yield return new WaitForSeconds(secondsToDelay);

        delayedAction.Invoke();
    }
    #endregion
}
