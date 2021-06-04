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

    public static bool actionConfirmation = true;
    public static bool manualMode = false;
    public static float volumeSlider = 1f;

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

    public void UpdateManualMode(Toggle toggle)
    {
        manualMode = toggle.isOn;
    }

    public void UpdateActionConfirmation(Toggle toggle)
    {
        actionConfirmation = toggle.isOn;
    }

    public void UpdateVolume(Slider slider)
    {
        volumeSlider = slider.value;
        GameAssetsManager.instance.SetVolume(slider.value);
    }
}
