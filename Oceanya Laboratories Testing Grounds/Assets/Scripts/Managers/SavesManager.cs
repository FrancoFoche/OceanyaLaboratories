using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SavesManager
{
    public static readonly string SAVE_FOLDER = Application.persistentDataPath + "/SaveFiles/";
    public static string SAVE_NAME = "save.txt";

    public static SaveFile loadedFile;
    public static void Initialize()
    {
        if (!Directory.Exists(SAVE_FOLDER))
        {
            Directory.CreateDirectory(SAVE_FOLDER);
        }

        Load();
    }

    public static void Save(SaveFile save)
    {
        string json = JsonUtility.ToJson(save);
        File.WriteAllText(SAVE_FOLDER + SAVE_NAME, json);
    }

    public static void Load()
    {
        if(File.Exists(SAVE_FOLDER + SAVE_NAME))
        {
            string save = File.ReadAllText(SAVE_FOLDER + SAVE_NAME);
            loadedFile = JsonUtility.FromJson<SaveFile>(save);
        }
        else
        {
            Debug.LogWarning("NO SAVE WAS FOUND");
            loadedFile = null;
        }
    }

    public static void DeleteSave()
    {
        if(File.Exists(SAVE_FOLDER + SAVE_NAME))
        {
            File.Delete(SAVE_FOLDER + SAVE_NAME);
        }
    }
}
[System.Serializable]
public class SaveFile
{
    public List<PlayerCharacter> players;
    public float volumeSliderValue;
    public bool actionConfirmation;
    public bool manualMode;
    public bool showOrderOfPlay;

    public PlayerCharacter FindPlayer(int id)
    {
        return players.Find(returner => returner.ID == id);
    }
}
