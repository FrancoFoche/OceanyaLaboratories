using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class MainMenu : MonoBehaviour
{
    public PlayableDirector director;
    [Tooltip("The time the script skips to in the timeline")]
    public float time;

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
}
