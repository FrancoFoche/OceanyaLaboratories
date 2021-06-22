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
public struct Wave
{
    public List<Character> allySide;
    public List<Character> enemySide;

    public Wave(List<Character> allySide, List<Character> enemySide)
    {
        this.allySide = allySide;
        this.enemySide = enemySide;
    }
}

public class BattleManager : MonoBehaviour
{
    private static BattleManager _instance;
    public static BattleManager i { get { if (_instance == null) { _instance = FindObjectOfType<BattleManager>(); } return _instance; } private set { _instance = value; } }


    public static   Character           caster              { get; private set; }
    public static   List<Character>     target              { get; private set; }

    public static   Wave[]              waves               { get; private set; }
    public static   int                 currentBattleIndex  { get; private set; }

    public          BattleState         battleState         { get; private set; }

    public          bool                inCombat            { get; private set; }
    public          bool                enemyTeamDeath      { get; private set; }
    public          bool                allyTeamDeath       { get; private set; }
    public          bool                debugMode           { get; private set; } //Toggles debug/manual battle features
    public          bool                confirmMode         { get; private set; } //Toggles confirmation popup

    public          BattleLog                   battleLog;
    public          BattleUIList                uiList;
    public          UICharacterActions          charActions;
    public          PauseMenu                   pauseMenu;
    public          TooltipPopup                tooltipPopup;
    public          UIMenu_TeamOrder            teamOrderMenu;
    public          UIActionConfirmationPopUp   confirmationPopup;

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
        if (loaded == null)
        {
            debugMode = MainMenu.manualMode;
            confirmMode = MainMenu.actionConfirmation;
            pauseMenu.volume.value = MainMenu.volume;
            teamOrderMenu.dropdownToggle.isOn = false;
        }
        else
        {
            debugMode = loaded.manualMode;
            confirmMode = loaded.actionConfirmation;
            pauseMenu.volume.value = loaded.volumeSliderValue;
            teamOrderMenu.dropdownToggle.isOn = loaded.showOrderOfPlay;
        }

        pauseMenu.manualMode.isOn = debugMode;
        pauseMenu.confirmActions.isOn = confirmMode;
        #endregion

        waves = new Wave[4];
        waves[0] = new Wave(
            new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9) },
            new List<Character>() { GameAssetsManager.instance.GetEnemy(1) }
            );
        waves[1] = new Wave(
            new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9) },
            new List<Character>() { GameAssetsManager.instance.GetEnemy(3) }
            );
        waves[2] = new Wave(
            new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9) },
            new List<Character>() { GameAssetsManager.instance.GetEnemy(1,1), GameAssetsManager.instance.GetEnemy(2), GameAssetsManager.instance.GetEnemy(1,2) }
            );
        waves[3] = new Wave(
            new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9) },
            new List<Character>() { GameAssetsManager.instance.GetEnemy(3,1), GameAssetsManager.instance.GetEnemy(1,1), GameAssetsManager.instance.GetEnemy(2), GameAssetsManager.instance.GetEnemy(1,2), GameAssetsManager.instance.GetEnemy(3,2) }
            );

        StartCombat(waves[0]);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (confirmationPopup.objToHide.activeSelf)
            {
                confirmationPopup.Hide();
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

        if (!pauseMenu.paused)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                teamOrderMenu.ToggleVisibility();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift))
                {
                    confirmationPopup.Show(delegate { SavesManager.DeleteSave(); SceneLoaderManager.instance.ReloadScene(); }, false, "Are you ABSOLUTELY sure that you want to delete your save file?");
                }
                else
                {
                    confirmationPopup.Show(() => SceneLoaderManager.instance.ReloadScene(), false, "Are you sure you want to restart the battle?");
                }
            }
            

            if (Input.GetKeyDown(KeyCode.M))
            {
                ToggleDebugMode();
            }

            if (TeamOrderManager.turnState == TurnState.WaitingForTarget || TeamOrderManager.turnState == TurnState.WaitingForConfirmation)
            {
                if (TeamOrderManager.turnState != TurnState.WaitingForConfirmation)
                {
                    if (Input.GetKeyDown(KeyCode.Return) || target.Count == UICharacterActions.instance.maxTargets)
                    {
                        Debug.Log("Targetting done.");
                        TeamOrderManager.SetTurnState(TurnState.WaitingForConfirmation);
                    }
                    else
                    {
                        if (caster.team == Team.Ally || debugMode)
                        {
                            SetTargets(uiList.CheckTargets());
                        }
                    }
                }

                if (Input.GetMouseButtonDown(1) && !TeamOrderManager.AIturn)
                {
                    UICharacterActions.instance.DenyAction();
                }
            }
            else
            {
                if (debugMode)
                {
                    uiList.CheckCurrentSelection();
                    bool togglesOn = uiList.toggleGroup.AnyTogglesOn();
                    if (togglesOn && uiList.different)
                    {
                        uiList.UpdateSelected();
                        GetCaster();
                        UISkillContext.instance.Hide();
                        UIItemContext.instance.Hide();
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

    void                            ToggleDebugMode     ()                          
    {
        if (debugMode)
        {
            SetDebugMode(false);
        }
        else
        {
            SetDebugMode(true);
        }
    }
    public void                     SetDebugMode        (bool mode)                 
    {
        debugMode = mode;
        battleLog.LogBattleEffect("Set Manual mode to " + mode);

        if (mode)
        {
            if (TeamOrderManager.turnState == TurnState.WaitingForAction)
            {
                uiList.InteractableUIs(true);
            }
        }
        else
        {
            uiList.SelectCharacter(TeamOrderManager.currentTurn);
        }
    }
    public void                     SetConfirmMode      (bool mode)                 
    {
        confirmMode = mode;

        if (mode)
        {
            battleLog.LogBattleEffect("Activated action confirmation.");
        }
        else
        {
            battleLog.LogBattleEffect("Action confirmation disabled.");
        }
    }
    public void                     StartCombat         (Wave combat)             
    {
        easteregg.gameObject.SetActive(false);
        UISkillContext.instance.Hide();
        UIItemContext.instance.Hide();

        enemyTeamDeath = false;
        allyTeamDeath = false;

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
                            TeamOrderManager.totalCharList[i].ActivatePassiveEffects(ActivationTime.StartOfBattle);
                        }

                        MusicManager.PlayMusic(Music.Combat);
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
                        MusicManager.PlayMusic(Music.Win);
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
                        MusicManager.PlayMusic(Music.Lose);
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
                            battleLog.LogCountdown(5, "Restarting battle in _countdown_...", () => StartCombat(waves[0]));
                            return;
                        }

                        battleState = BattleState.End;

                        int EXPgiven = 0;
                        List<Character> enemies = uiList.GetTeam(Team.Enemy);
                        for (int i = 0; i < enemies.Count; i++)
                        {
                            EXPgiven += Mathf.CeilToInt(enemies[i].stats.GetStat(Stats.MAXHP).value/2);
                        }

                        GiveAllyTeamEXP(EXPgiven);

                        DelayAction(5, delegate
                        {
                            currentBattleIndex++;
                            if (currentBattleIndex < waves.Length)
                            {
                                battleLog.LogCountdown(5, "Next wave starts in _countdown_...", () => StartCombat(waves[currentBattleIndex]));
                            }
                            else
                            {
                                battleLog.LogCountdown(10, "Credits start in _countdown_...", () => SceneLoaderManager.instance.LoadCredits());
                            }
                        });

                        SaveGame();
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
    public void                     SaveGame()
    {
        SaveFile save = new SaveFile()
        { 
            players = DBPlayerCharacter.pCharacters,
            actionConfirmation = confirmMode,
            manualMode = debugMode,
            volumeSliderValue = pauseMenu.volumeSliderValue,
            showOrderOfPlay = teamOrderMenu.dropdownToggle.isOn
        };
        SavesManager.Save(save);
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
