using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : MonoBehaviour
{
    /// <summary>
    /// Scenes and their build index
    /// </summary>
    public enum Scenes
    {
        MainMenu = 0,
        Combat = 1,
        Credits = 2,
        Instructions = 3
    }

    #region Setup
    private static SceneLoaderManager _instance;
    private static bool created = false; 
    public static SceneLoaderManager instance
    {
        get
        {
            if (!created)
            {
                SceneLoaderManager temp = FindObjectOfType<SceneLoaderManager>();

                if (temp == null)
                {
                    if (_instance == null)
                    {
                        _instance = (Instantiate(Resources.Load("SceneManager") as GameObject).GetComponent<SceneLoaderManager>());
                        created = true;
                    }
                }
                else
                {
                    _instance = temp;
                    created = true;
                }
            }
            
            return _instance;
        }
    }
    #endregion

    public void LoadScene(Scenes scene)
    {
        int newBuildIndex = (int)scene;

        if (SceneManager.GetActiveScene().buildIndex != newBuildIndex)
        {
            Destroy(MusicManager.musicObj);
            MusicManager.musicObj = null;
        }

        SceneManager.LoadScene(newBuildIndex);
    }

    public void LoadMainMenu()
    {
        LoadScene(Scenes.MainMenu);
    }

    public void LoadCredits()
    {
        LoadScene(Scenes.Credits);
    }

    public void LoadPlay()
    {
        LoadScene(Scenes.Combat);
    }

    public void LoadInstructions()
    {
        LoadScene(Scenes.Instructions);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        Debug.Log("Exitted the application");
        Application.Quit();
    }
}
