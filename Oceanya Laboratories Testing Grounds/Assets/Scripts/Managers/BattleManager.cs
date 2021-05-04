using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum BattleState
{
    Start,
    Won,
    Lost,
    End
}

public class BattleManager : MonoBehaviour
{
    public static   Character           caster;
    public static   List<Character>     target { get; private set; }

    public          BattleState         battleState;
    
    public          bool                inCombat;
    public          bool                enemyTeamDeath;
    public          bool                allyTeamDeath;

    public static   BattleLog           battleLog;
    public static   BattleUIList        uiList;
    public static   UICharacterActions  charActions;
    public static   BattleManager       instance;

    public          GameObject          easteregg;

    float exitTime = 1.5f;
    float curHold;

    private void Start()
    {
        easteregg.gameObject.SetActive(false);
        uiList = FindObjectOfType<BattleUIList>();
        battleLog = FindObjectOfType<BattleLog>();
        charActions = FindObjectOfType<UICharacterActions>();
        target = new List<Character>();
        caster = new Character();
        instance = this;

        TeamOrderManager.BuildTeamOrder();
        StartCombat();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (TeamOrderManager.turnState == TurnState.WaitingForTarget)
        {
            if (Input.GetKeyDown(KeyCode.Return) || target.Count == UICharacterActions.instance.maxTargets)
            {
                Debug.Log("Targetting done.");
                uiList.TurnToggles(false);
                UICharacterActions.instance.Act(caster, target);
            }
            else
            {
                target = uiList.CheckTargets();
            }
        }
        else
        {
            bool togglesOn = uiList.toggleGroup.AnyTogglesOn();
            if (togglesOn && uiList.different)
            {
                uiList.UpdateSelected();
                GetCaster();
                UISkillContext.instance.Hide();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && caster != TeamOrderManager.currentTurn)
        {
            battleLog.LogBattleEffect("The GM decided to revert back to the turn that was supposed to take place. Smh.");
            ReselectOriginalTurn();
        }
        

        if (Input.GetKey(KeyCode.Escape))
        {
            curHold += Time.deltaTime;

            if(curHold > exitTime)
            {
                Debug.Log("Exitted the application");
                Application.Quit();
            }
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            curHold = 0;
        }
    }


    public void                     StartCombat         ()                  
    {
        charActions.AddAllActions();
        SetBattleState(BattleState.Start);
        StartCoroutine(SetupBattle());
    }
    public IEnumerator              SetupBattle         ()                  
    {
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

        yield return new WaitForSeconds(3);

        caster = TeamOrderManager.spdSystem.teamOrder[0];
        TeamOrderManager.SetCurrentTurn(caster);
        caster.ActivatePassiveEffects(ActivationTime.StartOfTurn);
        TeamOrderManager.SetTurnState(TurnState.WaitingForAction);
    }           
    
    public void                     SetBattleState      (BattleState state) 
    {
        SpriteRenderer[] array = easteregg.GetComponentsInChildren<SpriteRenderer>();

        switch (state)
        {
            case BattleState.Start:
                battleState = BattleState.Start;
                MusicManager.PlayMusic(Music.Combat);
                charActions.InteractableButtons(false);
                uiList.SetInteractable(false);
                battleLog.LogBattleStatus("COMBAT START!");
                break;

            case BattleState.Won:
                battleState = BattleState.End;
                MusicManager.PlayMusic(Music.Win);
                charActions.InteractableButtons(false);
                uiList.SetInteractable(false);
                battleLog.LogBattleStatus("Ally team wins!");
                easteregg.SetActive(true);
                for (int i = 0; i < array.Length; i++)
                {
                    array[i].color = Color.green;
                }
                break;

            case BattleState.Lost:
                battleState = BattleState.End;
                MusicManager.PlayMusic(Music.Lose);
                charActions.InteractableButtons(false);
                uiList.SetInteractable(false);
                battleLog.LogBattleStatus("Enemy team wins!");
                easteregg.SetActive(true);
                for (int i = 0; i < array.Length; i++)
                {
                    array[i].color = Color.red;
                }
                break;
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
                SetBattleState(BattleState.Won);
                return;
            }
        }
    }
    public void                     GetCaster           ()                  
    {
        if (TeamOrderManager.currentTurn != BattleUIList.curCharacterSelected)
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
    public void                     ClearTargets        ()                  
    {
        target = new List<Character>();
    }
    public void                     ResetCheckedPassives()                  
    {
        caster.checkedPassives = false;

        for (int i = 0; i < target.Count; i++)
        {
            target[i].checkedPassives = false;
        }
    }
    public void                     ReselectOriginalTurn()                  
    {
        uiList.SelectCharacter(TeamOrderManager.currentTurn);
        battleLog.LogTurn(TeamOrderManager.currentTurn, 2);
    }
    
}
