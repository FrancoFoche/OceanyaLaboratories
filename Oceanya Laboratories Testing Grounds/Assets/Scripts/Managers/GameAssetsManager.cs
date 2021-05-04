using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Sounds
{
    AttackSlash,
    Hit,
    Buff,
    Debuff,
    Heal,
    Death
}

public enum Music
{
    MainMenu,
    Combat,
    Win,
    Lose,
    Credits
}

public enum Sprites
{
    MagnoDrip
}

public class GameAssetsManager : MonoBehaviour
{
    #region Setup
    private static GameAssetsManager _instance;
    public static GameAssetsManager instance { 
        get 
        { 
            if(_instance == null)
            {
                _instance = (Instantiate(Resources.Load("GameAssets") as GameObject).GetComponent<GameAssetsManager>());
            }

            return _instance;
        } 
    }
    #endregion

    #region Arrays/Databases

    public SpriteInfo[] sprites;
    public SoundAudioClip[] sounds;
    public MusicInfo[] music;

    #endregion

    #region Data Structures
    [System.Serializable] public struct SpriteInfo
    {
        public Sprites spriteName;
        public Sprite sprite;
    }
    [System.Serializable] public struct SoundAudioClip
    {
        public Sounds sound;
        public AudioClip audioClip;
    }
    [System.Serializable] public struct MusicInfo
    {
        public Music music;
        public AudioClip audioClip;
    }
    #endregion

    #region Getter Functions
    public Sprite GetSprite(Sprites sprite)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i].spriteName == sprite)
            {
                return sprites[i].sprite;
            }
        }

        Debug.LogError("Could not find the sprite " + sprite.ToString() + ".");
        return null;
    }
    public AudioClip GetAudioClip(Sounds sound)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].sound == sound)
            {
                return sounds[i].audioClip;
            }
        }

        Debug.LogError("Could not find the sound " + sound.ToString() + ".");
        return null;
    }
    public AudioClip GetMusic(Music music)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (this.music[i].music == music)
            {
                return this.music[i].audioClip;
            }
        }

        Debug.LogError("Could not find the sound " + music.ToString() + ".");
        return null;
    }
    #endregion
}
