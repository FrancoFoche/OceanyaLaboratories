﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
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

    public AudioMixer           mixer;
    public AudioMixerGroup      mixerMasterGroup;
    public SpriteInfo[]         sprites;
    public SoundAudioClip[]     sounds;
    public MusicInfo[]          music;

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

    public PlayerCharacter  GetPC           (int id)                                
    {
        for (int i = 0; i < DBPlayerCharacter.pCharacters.Count; i++)
        {
            PlayerCharacter current = DBPlayerCharacter.pCharacters[i];
            if (current.ID == id)
            {
                return current;
            }
        }

        Debug.LogError("Could not find the Player Character with id " + id + ".");
        return null;
    }
    public PlayerCharacter  GetPC           (string name)                           
    {
        for (int i = 0; i < DBPlayerCharacter.pCharacters.Count; i++)
        {
            PlayerCharacter current = DBPlayerCharacter.pCharacters[i];
            if (current.name == name)
            {
                return current;
            }
        }
        
        Debug.LogError("Could not find the Player Character with name " + name + ".");
        return null;
    }
    public Enemy            GetEnemy        (int id)                                
    {
        for (int i = 0; i < DBEnemies.enemies.Count; i++)
        {
            Enemy current = DBEnemies.enemies[i];
            if (current.ID == id)
            {
                return current;
            }
        }

        Debug.LogError("Could not find the enemy with id " + id + ".");
        return null;
    }
    public Enemy            GetEnemy        (string name)                           
    {
        for (int i = 0; i < DBEnemies.enemies.Count; i++)
        {
            Enemy current = DBEnemies.enemies[i];
            if (current.name == name)
            {
                return current;
            }
        }

        Debug.LogError("Could not find the enemy with name " + name + ".");
        return null;
    }
    public Item             GetItem         (int id)                                
    {
        for (int i = 0; i < DBItems.items.Count; i++)
        {
            Item current = DBItems.items[i];
            if (current.ID == id)
            {
                return current;
            }
        }

        Debug.LogError("Could not find the enemy with id " + id + ".");
        return null;
    }
    public Item             GetItem         (string name)                           
    {
        for (int i = 0; i < DBItems.items.Count; i++)
        {
            Item current = DBItems.items[i];
            if (current.name == name)
            {
                return current;
            }
        }
        
        Debug.LogError("Could not find the enemy with name " + name + ".");
        return null;
    }
    public BaseSkillClass   GetClass        (int id)                                
    {
        for (int i = 0; i < DBClasses.classes.Count; i++)
        {
            BaseSkillClass current = DBClasses.classes[i];
            if (current.ID == id)
            {
                return current;
            }
        }

        Debug.LogError("Could not find the class with id " + id + ".");
        return null;
    }
    public BaseSkillClass   GetClass        (string name)                           
    {
        for (int i = 0; i < DBClasses.classes.Count; i++)
        {
            BaseSkillClass current = DBClasses.classes[i];
            if (current.name == name)
            {
                return current;
            }
        }

        Debug.LogError("Could not find the class with name " + name + ".");
        return null;
    }   
    public Skill            GetSkill        (int classID, int skillID)              
    {
        for (int i = 0; i < DBSkills.skills.Count; i++)
        {
            Skill current = DBSkills.skills[i];
            if (current.skillClass.ID == classID && current.ID == skillID)
            {
                return current;
            }
        }

        Debug.LogError("Could not find the skill with class ID " + classID + " and skill ID " + skillID + ".");
        return null;
    }
    public Skill            GetSkill        (string className, string skillName)    
    {
        for (int i = 0; i < DBSkills.skills.Count; i++)
        {
            Skill current = DBSkills.skills[i];
            if (current.skillClass.name == className && current.name == skillName)
            {
                return current;
            }
        }

        Debug.LogError($"Could not find the skill '{skillName}' of class {className}.");
        return null;
    }
    public Sprite           GetSprite       (Sprites sprite)                        
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
    public AudioClip        GetAudioClip    (Sounds sound)                          
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
    public AudioClip        GetMusic        (Music music)                           
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

    public void SetVolume(float sliderValue)
    {
        mixer.SetFloat("GeneralVolume", Mathf.Log10(sliderValue) * 20);
    }
}
