using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// just a testing class, nothing here makes sense so don't worry about it
/// </summary>
public class TestingClass : MonoBehaviour
{
    LevellingSystem levelTest;
    PlayerCharacter character;

    private void Awake()
    {
        SavesManager.Initialize();
        DBClasses.BuildDatabase();
        LevellingSystem.BuildBaseLevelSystem();
    }

    private void Start()
    {
        
    }
    public void PrintChar(PlayerCharacter character)
    {
        Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        Debug.Log("Current EXP: " + character.level.EXP + ";");
        Debug.Log("Current Level: " + character.level.Level + ";");
        string stats = "Stats: ";


        for (int i = 0; i < character.stats.Count; i++)
        {
            Stats curStat = character.stats[i].stat;
            int curValue = character.stats[i].value;
            stats += $"\n {curStat} | {curValue}";
        }

        Debug.Log(stats);
        Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
    }
}
