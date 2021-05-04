using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DataBase Order decides order in which data is built, since some database require others to be loaded first before being able to build themselves properly.
/// </summary>
public class DataBaseOrder : MonoBehaviour
{
    public static List<DataBaseOrder> databases;

    private void Awake()
    {
        RuleManager.BuildHelpers();
        DBClasses.BuildDatabase();
        DBSkills.BuildDatabase();
        DBPlayerCharacter.BuildDatabase();
        DBEnemies.BuildDatabase();
    }
}