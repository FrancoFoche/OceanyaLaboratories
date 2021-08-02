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
        public Music levelMusic;
        public SceneLoaderManager.Scenes winningScene;
        public Wave[] waves;
    }

    public static int currentLevel;
    public static int lastClearedLevel;
    public static BattleLevel[] levels;

    public Color level_AlreadyPassed;
    public Color level_Locked;

    private static void CreateLevels()
    {
        levels =
        new BattleLevel[]
        {
            new BattleLevel
            {
                levelNumber = 0,
                name = "Tutorial",
                description = "Get eased in and start your first fight with Oceanya's Warriors!",
                levelMusic = Music.Combat,
                winningScene = SceneLoaderManager.Scenes.MainMenu,
                waves =
                new Wave[]
                {
                    new Wave()
                    {
                        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9), GameAssetsManager.instance.GetPC(101) },
                        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(1) }
                    },
                    new Wave()
                    {
                        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9), GameAssetsManager.instance.GetPC(101) },
                        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(3) }
                    },
                    new Wave()
                    {
                        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9), GameAssetsManager.instance.GetPC(101) },
                        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(2) }
                    },
                }
            },
            new BattleLevel
            {
                levelNumber = 1,
                name = "Swordsmaster's Challenge",
                description = "Your first challenge, it's just two waves, you can do it!",
                levelMusic = Music.Combat,
                winningScene = SceneLoaderManager.Scenes.MainMenu,
                waves =
                new Wave[]
                {
                    new Wave()
                    {
                        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9), GameAssetsManager.instance.GetPC(101) },
                        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(1, 1), GameAssetsManager.instance.GetEnemy(2), GameAssetsManager.instance.GetEnemy(1, 2) }
                    },
                    new Wave()
                    {
                        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9), GameAssetsManager.instance.GetPC(101) },
                        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(3, 1), GameAssetsManager.instance.GetEnemy(1, 1), GameAssetsManager.instance.GetEnemy(2), GameAssetsManager.instance.GetEnemy(1, 2), GameAssetsManager.instance.GetEnemy(3, 2) }
                    },
                }
            },
            new BattleLevel
            {
                levelNumber = 2,
                name = "Oceanya's Champion",
                description = "One fight, One enemy. One you. Good luck.",
                levelMusic = Music.Combat,
                winningScene = SceneLoaderManager.Scenes.MainMenu,
                waves =
                new Wave[]
                {
                    new Wave()
                    {
                        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9), GameAssetsManager.instance.GetPC(101) },
                        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(666) },
                        winMusicPlay = false,
                        lossMusicPlay = false,
                        waveMusic = Music.GarouTheme
                    },
                }
            },
            new BattleLevel
            {
                levelNumber = 3,
                name = "Oceanya's Pantheon.",
                description = "Oceanya's ultimate challenge, beat every single fight in a row, no breaks. Show us what you're made of! (This level is entirely optional)",
                levelMusic = Music.GenosTheme,
                winningScene = SceneLoaderManager.Scenes.Credits,
                waves =
                new Wave[]
                {
                    new Wave()
                    {
                        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9), GameAssetsManager.instance.GetPC(101) },
                        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(1) },
                        winMusicPlay = false,
                        lossMusicPlay = false,
                        transitionOutTime = 0,
                    },
                    new Wave()
                    {
                        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9), GameAssetsManager.instance.GetPC(101) },
                        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(3) },
                        winMusicPlay = false,
                        lossMusicPlay = false,
                        transitionOutTime = 0,
                    },
                    new Wave()
                    {
                        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9), GameAssetsManager.instance.GetPC(101) },
                        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(2) },
                        winMusicPlay = false,
                        lossMusicPlay = false,
                        transitionOutTime = 0,
                    },
                    new Wave()
                    {
                        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9), GameAssetsManager.instance.GetPC(101) },
                        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(1, 1), GameAssetsManager.instance.GetEnemy(2), GameAssetsManager.instance.GetEnemy(1, 2) },
                        winMusicPlay = false,
                        lossMusicPlay = false,
                        transitionOutTime = 0,
                    },
                    new Wave()
                    {
                        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9), GameAssetsManager.instance.GetPC(101) },
                        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(3, 1), GameAssetsManager.instance.GetEnemy(1, 1), GameAssetsManager.instance.GetEnemy(2), GameAssetsManager.instance.GetEnemy(1, 2), GameAssetsManager.instance.GetEnemy(3, 2) },
                        winMusicPlay = false,
                        lossMusicPlay = false,
                        transitionOutTime = 0,
                    },
                    new Wave()
                    {
                        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9), GameAssetsManager.instance.GetPC(101) },
                        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(666) },
                        winMusicPlay = false,
                        lossMusicPlay = false,
                        transitionOutTime = 5,
                    },
                }
            }
        };
    }

    public void LoadLevels()
    {
        #region Savefile loading
        SaveFile loaded = SavesManager.loadedFile;
        if (loaded == null)
        {
            Debug.Log("Save was null");
            lastClearedLevel = -1;
        }
        else
        {
            Debug.Log("Save was NOT null");
            lastClearedLevel = loaded.lastLevelCleared;
        }
        #endregion

        CreateLevels();
        ClearList();

        Debug.Log("Last cleared level: " + lastClearedLevel);

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

    public static BattleLevel GetLevel(int levelNumber)
    {
        CreateLevels();
        for (int i = 0; i < levels.Length; i++)
        {
            if(levelNumber == levels[i].levelNumber)
            {
                return levels[i];
            }
        }

        throw new System.Exception("Incorrect level number! (" + levelNumber +")");
    }
}
