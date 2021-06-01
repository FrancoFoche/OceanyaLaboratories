using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public static BattleManager instance { get { if (_instance == null) { _instance = FindObjectOfType<BattleManager>(); } return _instance; } private set { _instance = value; } }


    public static   Character           caster          { get; private set; }
    public static   List<Character>     target          { get; private set; }

    public          BattleState         battleState     { get; private set; }

    public          bool                inCombat        { get; private set; }
    public          bool                enemyTeamDeath  { get; private set; }
    public          bool                allyTeamDeath   { get; private set; }

    public static   BattleLog           battleLog       { get; private set; }
    public static   BattleUIList        uiList          { get; private set; }
    public static   UICharacterActions  charActions     { get; private set; }
    
    public          bool                debugMode       { get; private set; } //Toggles debug/manual battle features

    public bool scriptableObjectMode;
    public          GameObject          easteregg;

    float exitTime = 1.5f;
    float curHold;

    private void Start()
    {
        easteregg.gameObject.SetActive(false);

        #region Initializations
        caster = ScriptableObject.CreateInstance<Character>();
        target = new List<Character>();

        //BattleState is initialized in "Start Combat"
        inCombat = false;
        enemyTeamDeath = false;
        allyTeamDeath = false;

        battleLog = FindObjectOfType<BattleLog>();
        uiList = FindObjectOfType<BattleUIList>();
        charActions = FindObjectOfType<UICharacterActions>();
        SetDebugMode(false);

        //easter egg is assigned through the editor
        #endregion

        TeamOrderManager.BuildTeamOrder();
        StartCombat();
    }

    private void Update()
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
                UICharacterActions.instance.ConfirmAction(caster, target);
                uiList.TurnToggles(false);
                target.Clear();
            }
            else
            {
                if(caster.team == Team.Ally || debugMode)
                {
                    SetTargets(uiList.CheckTargets());
                }
            }
        }
        else
        {
            if (debugMode)
            {
                bool togglesOn = uiList.toggleGroup.AnyTogglesOn();
                if (togglesOn && uiList.different)
                {
                    uiList.UpdateSelected();
                    GetCaster();
                    UISkillContext.instance.Hide();
                }
            }
            else
            {
                if(BattleUIList.curCharacterSelected != TeamOrderManager.currentTurn && battleState == BattleState.InCombat)
                {
                    Debug.LogWarning("The half-assed bugfix patch was triggered.");
                    uiList.SelectCharacter(TeamOrderManager.currentTurn);
                }

            }
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            curHold += Time.deltaTime;

            if(curHold > exitTime)
            {
                SceneLoaderManager.instance.Quit(); 
            }
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            curHold = 0;
        }

        #region Debug Features
        if (Input.GetKeyDown(KeyCode.Escape) && caster != TeamOrderManager.currentTurn)
        {
            battleLog.LogBattleEffect("The GM decided to revert back to the turn that was supposed to take place. Smh.");
            ReselectOriginalTurn();
        }
        #endregion
    }

    void                            ToggleDebugMode     ()                          
    {
        battleLog.LogBattleEffect("Set debug mode to " + !debugMode + ".");

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

        if (TeamOrderManager.turnState == TurnState.WaitingForAction && mode)
        {
            uiList.InteractableToggles(true);
            battleLog.LogBattleEffect("Set UIs to interactable.");
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
                        uiList.InteractableToggles(false);
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
                        uiList.InteractableToggles(false);
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
                        uiList.InteractableToggles(false);
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
        target = targets;
        Debug.Log("Set targets, current count: " + target.Count);
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
