using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public bool paused;
    public Utilities_Button_BinarySprite buttonSprite;
    public GameObject pauseRoot;
    public GameObject settingsRoot;
    public Toggle manualMode;
    public Toggle confirmActions;
    public Slider volume;
    public GameObject generalRoot;

    public void TogglePause()
    {
        if (paused)
        {
            Hide();
        }
        else
        {
            Show();
        }

        buttonSprite.BinaryToggleSprite();
    }

    public void Show()
    {
        paused = true;
        SettingsManager.LoadSettings(SavesManager.loadedFile);
        LoadSettings();

        if (settingsRoot != null)
        {
            settingsRoot.SetActive(false);
        }

        if (generalRoot != null)
        {
            generalRoot.SetActive(true);
        }

        if (pauseRoot != null)
        {
            pauseRoot.SetActive(true);
        }
    }

    public void Hide()
    {
        paused = false;
        pauseRoot.SetActive(false);
        UIActionConfirmationPopUp.i?.Hide();
        SettingsManager.SaveSettings();
    }

    public void VolumeSlider(Slider volumeSlider)
    {
        SettingsManager.volume = volumeSlider.value;
        GameAssetsManager.instance.SetVolume(volumeSlider.value);
    }

    public void SettingsButton()
    {
        if (settingsRoot != null)
        {
            settingsRoot.SetActive(true);
        }

        if (generalRoot != null)
        {
            generalRoot.SetActive(false);
        }
    }

    public void SettingsBackButton()
    {
        if (settingsRoot != null)
        {
            settingsRoot.SetActive(false);
        }

        if (generalRoot != null)
        {
            generalRoot.SetActive(true);
        }

        SettingsManager.SaveSettings();
    }

    public void SaveSettings()
    {
        SettingsManager.manualMode = manualMode.isOn;
        SettingsManager.actionConfirmation = confirmActions.isOn;
        SettingsManager.volume = volume.value;

        SettingsManager.SaveSettings();
    }
    public void LoadSettings()
    {
        manualMode.isOn = SettingsManager.manualMode;
        confirmActions.isOn = SettingsManager.actionConfirmation;
        volume.value = SettingsManager.volume;
    }
    public void SetDebugMode()
    {
        SettingsManager.SetDebugMode(manualMode.isOn);
    }
    public void SetConfirmMode()
    {
        SettingsManager.SetConfirmMode(confirmActions.isOn);
    }

    public void Quit()
    {
        UIActionConfirmationPopUp.i.Show(SceneLoaderManager.instance.Quit, false, "Are you sure you want to quit?");
    }
    public void MainMenu()
    {
        UIActionConfirmationPopUp.i.Show(SceneLoaderManager.instance.LoadMainMenu, false, "Are you sure you want to head to the Main Menu?");
    }
    public void Restart()
    {
        UIActionConfirmationPopUp.i.Show(SceneLoaderManager.instance.ReloadScene, false, "Are you sure you want to restart?");
    }
}
