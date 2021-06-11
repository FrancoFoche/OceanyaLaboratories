using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DataBase Order decides order in which data is built, since some database require others to be loaded first before being able to build themselves properly.
/// </summary>
public class DataBaseOrder : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("Building Databases");
        RuleManager.BuildHelpers();
        DBClasses.BuildDatabase();
        DBSkills.BuildDatabase();
        DBItems.BuildDatabase();
        LevellingSystem.BuildBaseLevelSystem();
        DBPlayerCharacter.BuildDatabase();
        DBEnemies.BuildDatabase();
    }
}
