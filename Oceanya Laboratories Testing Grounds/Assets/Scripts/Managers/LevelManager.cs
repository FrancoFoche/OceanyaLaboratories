using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : ButtonList
{
    public class BattleLevel
    {
        public int levelNumber;
        public string name;
        public string description;
        public Wave[] waves;
    }

    public static int currentLevel;
    public static int lastClearedLevel;
    public static BattleLevel[] levels;

    public Color level_AlreadyPassed;
    public Color level_Locked;

    private void Awake()
    {
        levels =
        new BattleLevel[]
        {
            new BattleLevel
            {
                levelNumber = 0,
                name = "Tutorial",
                description = "Get eased in and start your first fight with Oceanya's Warriors!",
                waves =
                new Wave[]
                {
                    new Wave(
                        new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9) },
                        new List<Character>() { GameAssetsManager.instance.GetEnemy(1) }
                            ),
                    new Wave(
                        new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9) },
                        new List<Character>() { GameAssetsManager.instance.GetEnemy(3) }
                            ),
                    new Wave(
                        new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9) },
                        new List<Character>() { GameAssetsManager.instance.GetEnemy(2) }
                            )
                }
            },
            new BattleLevel
            {
                levelNumber = 1,
                name = "Swordsmaster's Challenge",
                description = "",
                waves =
                new Wave[]
                {
                    new Wave(
                        new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9) },
                        new List<Character>() { GameAssetsManager.instance.GetEnemy(1, 1), GameAssetsManager.instance.GetEnemy(2), GameAssetsManager.instance.GetEnemy(1, 2) }
                            ),
                    new Wave(
                        new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9) },
                        new List<Character>() { GameAssetsManager.instance.GetEnemy(3, 1), GameAssetsManager.instance.GetEnemy(1, 1), GameAssetsManager.instance.GetEnemy(2), GameAssetsManager.instance.GetEnemy(1, 2), GameAssetsManager.instance.GetEnemy(3, 2) }
                            )
                }
            },
            new BattleLevel
            {
                levelNumber = 2,
                name = "Test",
                description = "",
                waves =
                new Wave[]
                {
                    new Wave(
                        new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9) },
                        new List<Character>() { GameAssetsManager.instance.GetEnemy(1, 1), GameAssetsManager.instance.GetEnemy(2), GameAssetsManager.instance.GetEnemy(1, 2) }
                            ),
                    new Wave(
                        new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9) },
                        new List<Character>() { GameAssetsManager.instance.GetEnemy(3, 1), GameAssetsManager.instance.GetEnemy(1, 1), GameAssetsManager.instance.GetEnemy(2), GameAssetsManager.instance.GetEnemy(1, 2), GameAssetsManager.instance.GetEnemy(3, 2) }
                            )
                }
            },
        };
    }
    private void Start()
    {
        #region Savefile loading
        SaveFile loaded = SavesManager.loadedFile;
        if (loaded == null)
        {
            lastClearedLevel = -1;
        }
        else
        {
            lastClearedLevel = loaded.lastLevelCleared;
        }
        #endregion
    }

    public void LoadLevels()
    {
        ClearList();

        for (int i = 0; i < levels.Length; i++)
        {
            AddLevel(levels[i]);
        }
    }

    public void AddLevel(BattleLevel level)
    {
        GameObject newEntry = AddObject();
        UILevelButton script = newEntry.GetComponent<UILevelButton>();
        script.LoadLevel(level);
        Button newButton = newEntry.GetComponent<Button>();
        buttons.Add(newButton);

        if(level.levelNumber <= lastClearedLevel)
        {
            script.ActivateColorOverlay(level_AlreadyPassed);
        }
        else if (level.levelNumber > lastClearedLevel + 1)
        {
            newButton.interactable = false;
            script.ActivateColorOverlay(level_Locked);
        }
    }

    public static Wave[] GetLevelWaves(int levelNumber)
    {
        for (int i = 0; i < levels.Length; i++)
        {
            if(levelNumber == levels[i].levelNumber)
            {
                return levels[i].waves;
            }
        }

        throw new System.Exception("Incorrect level number! (" + levelNumber +")");
    }
}
