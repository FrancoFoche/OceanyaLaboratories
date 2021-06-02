using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public bool paused;
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
    }

    public void Show()
    {
        paused = true;
        settingsRoot.SetActive(false);
        generalRoot.SetActive(true);
        pauseRoot.SetActive(true);
    }

    public void Hide()
    {
        paused = false;
        pauseRoot.SetActive(false);
    }

    public void VolumeSlider(float sliderValue)
    {
        GameAssetsManager.instance.SetVolume(sliderValue);
    }

    public void SettingsButton()
    {
        settingsRoot.SetActive(true);
        UpdateValues();
        generalRoot.SetActive(false);
    }

    public void SettingsBackButton()
    {
        settingsRoot.SetActive(false);
        generalRoot.SetActive(true);
    }

    public void UpdateValues()
    {
        manualMode.isOn = BattleManager.i.debugMode;
        confirmActions.isOn = BattleManager.i.confirmMode;
    }
    public void SetDebugMode()
    {
        BattleManager.i.SetDebugMode(manualMode.isOn);
    }
    public void SetConfirmMode()
    {
        BattleManager.i.SetConfirmMode(confirmActions.isOn);
    }

    public void Quit()
    {
        UICharacterActions.instance.confirmationPopup.Show(SceneLoaderManager.instance.Quit, false);
    }
    public void MainMenu()
    {
        UICharacterActions.instance.confirmationPopup.Show(SceneLoaderManager.instance.LoadMainMenu, false);
    }
    public void Restart()
    {
        UICharacterActions.instance.confirmationPopup.Show(SceneLoaderManager.instance.ReloadScene, false);
    }
}
