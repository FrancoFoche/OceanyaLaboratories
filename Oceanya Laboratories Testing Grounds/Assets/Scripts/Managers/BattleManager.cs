using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    Start,
    InCombat,
    Won,
    Lost,
    End
}

public class BattleManager : MonoBehaviour
{
    private static BattleManager _instance;
    public static BattleManager i { get { if (_instance == null) { _instance = FindObjectOfType<BattleManager>(); } return _instance; } private set { _instance = value; } }


    public static   Character           caster          { get; private set; }
    public static   List<Character>     target          { get; private set; }

    public          BattleState         battleState     { get; private set; }

    public          bool                inCombat        { get; private set; }
    public          bool                enemyTeamDeath  { get; private set; }
    public          bool                allyTeamDeath   { get; private set; }
    public          bool                debugMode       { get; private set; } //Toggles debug/manual battle features
    public          bool                confirmMode     { get; private set; } //Toggles confirmation popup

    public          BattleLog           battleLog;
    public          BattleUIList        uiList;
    public          UICharacterActions  charActions;
    public          PauseMenu           pauseMenu;

    public          GameObject          easteregg;

    public UnityEngine.UI.Toggle test;

    float exitTime = 1.5f;
    float curHold;

    private void Start()
    {
        easteregg.gameObject.SetActive(false);

        #region Initializations
        caster = new Character();
        target = new List<Character>();

        //BattleState is initialized in "Start Combat"
        inCombat = false;
        enemyTeamDeath = false;
        allyTeamDeath = false;

        pauseMenu.manualMode.isOn = false;
        debugMode = false;

        confirmMode = true;
        pauseMenu.confirmActions.isOn = true;

        //easter egg is assigned through the editor
        #endregion

        TeamOrderManager.BuildTeamOrder();
        StartCombat();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            test.isOn = !test.isOn;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && TeamOrderManager.turnState == TurnState.WaitingForAction)
        {
            pauseMenu.TogglePause();
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
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneLoaderManager.instance.ReloadScene();
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                ToggleDebugMode();
            }

            if (TeamOrderManager.turnState == TurnState.WaitingForTarget)
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

                if (Input.GetKeyDown(KeyCode.Escape) && !TeamOrderManager.AIturn)
                {
                    UICharacterActions.instance.confirmationPopup.Deny();
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
            //uiList.SelectCharacter(TeamOrderManager.currentTurn);
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
    public void                     StartCombat         ()                          
    {
        charActions.AddAllActions();
        SetBattleState(BattleState.Start);
        DelayAction(3,SetupBattle);
    }
    public void                     SetupBattle         ()                          
    {
        caster = TeamOrderManager.spdSystem.teamOrder[0];
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

                        for (int i = 0; i < TeamOrderManager.allySide.Count; i++)
                        {
                            uiList.AddAlly(TeamOrderManager.allySide[i]);
                        }

                        EnemySpawner.instance.SpawnAllEnemies(TeamOrderManager.enemySide);

                        for (int i = 0; i < TeamOrderManager.totalCharList.Count; i++)
                        {
                            TeamOrderManager.totalCharList[i].CheckPassives();
                            TeamOrderManager.totalCharList[i].ActivatePassiveEffects(ActivationTime.StartOfBattle);
                        }

                        MusicManager.PlayMusic(Music.Combat);
                        charActions.InteractableButtons(false);
                        uiList.InteractableUIs(false);
                        battleLog.LogBattleStatus("COMBAT START!");
                    }
                    break;

                case BattleState.InCombat:
                    inCombat = true;
                    battleState = BattleState.InCombat;
                    break;

                case BattleState.Won:
                    {
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
                    battleState = BattleState.End;
                    battleLog.LogCountdown(20, "Credits start in _countdown_...", () => SceneLoaderManager.instance.LoadCredits());
                    break;
            }
        }
    }
    public void                     CheckTotalTeamKill  ()                          
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
                SetBattleState(BattleState.Lost);
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

    public void                     DelayAction         (float secondsToDelay, Action delayedAction)      
    {
        StartCoroutine(Delay(secondsToDelay, delayedAction));
    }

    IEnumerator                     Delay               (float secondsToDelay, Action delayedAction)
    {
        yield return new WaitForSeconds(secondsToDelay);

        delayedAction.Invoke();
    }
}
