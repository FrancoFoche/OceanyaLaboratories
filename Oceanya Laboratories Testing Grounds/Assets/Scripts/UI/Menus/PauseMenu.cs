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
    public float volumeSliderValue;
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
        settingsRoot.SetActive(false);
        generalRoot.SetActive(true);
        pauseRoot.SetActive(true);
    }

    public void Hide()
    {
        paused = false;
        pauseRoot.SetActive(false);
        BattleManager.i.confirmationPopup.Hide();
        BattleManager.i.SaveGame();
    }

    public void VolumeSlider(float sliderValue)
    {
        volumeSliderValue = sliderValue;
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
        BattleManager.i.confirmationPopup.Show(SceneLoaderManager.instance.Quit, false, "Are you sure you want to quit?");
    }
    public void MainMenu()
    {
        BattleManager.i.confirmationPopup.Show(SceneLoaderManager.instance.LoadMainMenu, false, "Are you sure you want to head to the Main Menu?");
    }
    public void Restart()
    {
        BattleManager.i.confirmationPopup.Show(SceneLoaderManager.instance.ReloadScene, false, "Are you sure you want to restart?");
    }
}
