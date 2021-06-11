using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SavesManager
{
    public static readonly string SAVE_FOLDER = Application.dataPath + "/SaveFiles/";
    public static string SAVE_NAME = "save.txt";

    public static void Initialize()
    {
        if (!Directory.Exists(SAVE_FOLDER))
        {
            Directory.CreateDirectory(SAVE_FOLDER);
        }
    }

    public static void Save(SaveFile save)
    {
        string json = JsonUtility.ToJson(save);
        File.WriteAllText(SAVE_FOLDER + SAVE_NAME, json);
    }

    public static SaveFile Load()
    {
        if(File.Exists(SAVE_FOLDER + SAVE_NAME))
        {
            string save = File.ReadAllText(SAVE_FOLDER + SAVE_NAME);
            SaveFile loaded = JsonUtility.FromJson<SaveFile>(save);
            return loaded;
        }
        else
        {
            Debug.LogWarning("NO SAVE WAS FOUND");
            return null;
        }
    }
}
[System.Serializable]
public class SaveFile
{
    public List<PlayerCharacter> players;
    public int test = 1;
}
