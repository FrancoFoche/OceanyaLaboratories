using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public PlayableDirector director;
    [Tooltip("The time the script skips to in the timeline")]
    public float time;

    
    public Toggle actionConfirmationToggle;
    public Toggle manualModeToggle;
    public Slider volumeSlider;

    private void Start()
    {
        PhotonNetwork.Disconnect();
        DataBaseOrder.i.Initialize();
        SavesManager.Load();
        SaveFile loaded = SavesManager.loadedFile;
        SettingsManager.LoadSettings(loaded);

        manualModeToggle.isOn = SettingsManager.manualMode;
        actionConfirmationToggle.isOn = SettingsManager.actionConfirmation;
        volumeSlider.value = SettingsManager.volume;
        GameAssetsManager.instance.SetVolume(SettingsManager.volume);
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
        SettingsManager.manualMode = manualModeToggle.isOn;
    }

    public void UpdateActionConfirmation()
    {
        SettingsManager.actionConfirmation = actionConfirmationToggle.isOn;
    }

    public void UpdateVolume()
    {
        SettingsManager.volume = volumeSlider.value;
        GameAssetsManager.instance.SetVolume(SettingsManager.volume);
    }

    public void SaveGameOnMenu()
    {
        SettingsManager.SaveSettings();
    }
}
