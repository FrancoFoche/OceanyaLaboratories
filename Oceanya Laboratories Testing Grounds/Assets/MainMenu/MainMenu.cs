using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public PlayableDirector director;
    [Tooltip("The time the script skips to in the timeline")]
    public float time;

    
    public Toggle actionConfirmationToggle;
    public static bool actionConfirmation = true;
    public Toggle manualModeToggle;
    public static bool manualMode = false;
    public Slider volumeSlider;
    public static float volume = 1f;

    private void Start()
    {
        DataBaseOrder.i.Initialize();
        SavesManager.Load();
        SaveFile loaded = SavesManager.loadedFile;
        if(loaded != null)
        {
            manualMode = loaded.manualMode;
            actionConfirmation = loaded.actionConfirmation;
            volume = loaded.volumeSliderValue;
        }

        manualModeToggle.isOn = manualMode;
        actionConfirmationToggle.isOn = actionConfirmation;
        volumeSlider.value = volume;
        GameAssetsManager.instance.SetVolume(volume);
    }
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if(director.time < time)
            {
                director.time = time;
            }
        }
    }

    public void UpdateManualMode()
    {
        manualMode = manualModeToggle.isOn;
    }

    public void UpdateActionConfirmation()
    {
        actionConfirmation = actionConfirmationToggle.isOn;
    }

    public void UpdateVolume()
    {
        volume = volumeSlider.value;
        GameAssetsManager.instance.SetVolume(volume);
    }

    public void SaveGameOnMenu()
    {
        SaveFile save = new SaveFile() 
        { 
            players = DBPlayerCharacter.pCharacters, 
            actionConfirmation = actionConfirmation, 
            manualMode = manualMode, 
            volumeSliderValue = volume,
            showOrderOfPlay = SavesManager.loadedFile == null ? true : SavesManager.loadedFile.showOrderOfPlay,
            orderOfPlay_showDead = SavesManager.loadedFile == null ? true : SavesManager.loadedFile.orderOfPlay_showDead,
            orderOfPlay_showPast = SavesManager.loadedFile == null ? true : SavesManager.loadedFile.orderOfPlay_showPast,
            lastLevelCleared = SavesManager.loadedFile == null ? -1 : SavesManager.loadedFile.lastLevelCleared,
        };
        SavesManager.Save(save);
    }
}
