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

    private void Start()
    {
        SavesManager.Initialize();
        DBClasses.BuildDatabase();
        LevellingSystem.BuildBaseLevelSystem();
        character = new PlayerCharacter(0, "TestName", 1, GameAssetsManager.instance.GetClass(ClassNames.Vampire.ToString()),
            new Dictionary<Stats, int>()
            {
                { Stats.MAXHP       , 1 },
                { Stats.CURHP       , 1 },
                { Stats.STR         , 1 },
                { Stats.INT         , 1 },
                { Stats.CHR         , 1 },
                { Stats.AGI         , 1 },
                { Stats.MR          , 0 },
                { Stats.PR          , 0 },
                { Stats.CON         , 1 },
                { Stats.HPREGEN     , 0 }
            },
            new List<Skill>()
            , new Dictionary<Item, int>());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            character.AddExp(20);
            Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Debug.Log("Current EXP: " + character.level.EXP + ";");
            Debug.Log("Current Level: " + character.level.Level + ";");
            string stats = "Stats: ";

            foreach (var kvp in character.stats)
            {
                stats += $"\n {kvp.Key} | {kvp.Value}";
            }

            Debug.Log(stats);
            Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveFile file = new SaveFile() { players = new List<PlayerCharacter>() { character } };

            SavesManager.Save(file);
            Debug.Log("Saved");
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            SaveFile loaded = SavesManager.Load();
            if(loaded != null)
            {
                character = loaded.players[0];
                Debug.Log("Loaded");
            }
            
        }
    }
}
