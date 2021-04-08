using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBase : MonoBehaviour
{
    /// <summary>
    /// DataBase decides order in which databases are built, since some database require others to be loaded first before being able to build themselves properly.
    /// </summary>
    
    private void Awake()
    {
        DBClasses.BuildDatabase();
        DBSkills.BuildDatabase();
        DBPlayerCharacter.BuildDatabase();
    }
}
