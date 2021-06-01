using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SFXManager
{
    public static GameObject soundObj;
    public static AudioSource source;
    public static void PlaySound(Sounds sound)
    {
        if(soundObj == null)
        {
            soundObj = new GameObject("Sound");
            source = soundObj.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = GameAssetsManager.instance.mixerMasterGroup;
        }
        
        source.PlayOneShot(GameAssetsManager.instance.GetAudioClip(sound));
    }
}

public static class MusicManager
{
    public static GameObject musicObj;
    public static AudioSource source;
    public static void PlayMusic(Music music)
    {
        if (musicObj == null)
        {
            musicObj = new GameObject("Music");
            source = musicObj.AddComponent<AudioSource>();
            source.loop = true;
            source.outputAudioMixerGroup = GameAssetsManager.instance.mixerMasterGroup;
        }

        source.clip = GameAssetsManager.instance.GetMusic(music);
        source.Play();
    }
}