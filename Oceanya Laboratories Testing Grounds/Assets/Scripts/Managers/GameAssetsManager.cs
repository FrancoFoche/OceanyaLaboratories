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

                if(_instance.skills == null)
                {
                    _instance.skills = new List<Skill>();
                }

                for (int i = 0; i < _instance.classes.Length; i++)
                {
                    BaseSkillClass currentClass = _instance.classes[i];

                    for (int j = 0; j < currentClass.skillList.Count; j++)
                    {
                        Skill currentSkill = currentClass.skillList[j];
                        currentSkill.skillClass = currentClass;
                        _instance.skills.Add(currentSkill);
                    }
                }
            }

            return _instance;
        } 
    }
    #endregion

    #region Arrays/Databases


    public PlayerCharacter[]    playerCharacters;
    public Enemy[]              enemies;
    public Item[]               items;
    public BaseSkillClass[]     classes;
    private List<Skill>         skills;
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
        for (int i = 0; i < playerCharacters.Length; i++)
        {
            if (playerCharacters[i].ID == id)
            {
                return playerCharacters[i];
            }
        }

        Debug.LogError("Could not find the Player Character with id " + id + ".");
        return null;
    }
    public PlayerCharacter  GetPC           (string name)   
    {
        for (int i = 0; i < playerCharacters.Length; i++)
        {
            if (playerCharacters[i].name == name)
            {
                return playerCharacters[i];
            }
        }

        Debug.LogError("Could not find the Player Character with name " + name + ".");
        return null;
    }
    public Enemy            GetEnemy        (int id)        
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].ID == id)
            {
                return enemies[i];
            }
        }

        Debug.LogError("Could not find the enemy with id " + id + ".");
        return null;
    }
    public Enemy            GetEnemy        (string name)   
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].name == name)
            {
                return enemies[i];
            }
        }

        Debug.LogError("Could not find the enemy with name " + name + ".");
        return null;
    }
    public Item             GetItem         (int id)                                
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].baseInfo.id == id)
            {
                return items[i];
            }
        }

        Debug.LogError("Could not find the enemy with id " + id + ".");
        return null;
    }
    public Item             GetItem         (string name)                           
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].baseInfo.name == name)
            {
                return items[i];
            }
        }

        Debug.LogError("Could not find the enemy with name " + name + ".");
        return null;
    }
    public BaseSkillClass   GetClass        (int id)                                
    {
        for (int i = 0; i < classes.Length; i++)
        {
            if (classes[i].baseInfo.id == id)
            {
                return classes[i];
            }
        }

        Debug.LogError("Could not find the class with id " + id + ".");
        return null;
    }
    public BaseSkillClass   GetClass        (string name)                           
    {
        for (int i = 0; i < classes.Length; i++)
        {
            if (classes[i].baseInfo.name == name)
            {
                return classes[i];
            }
        }

        Debug.LogError("Could not find the class with name " + name + ".");
        return null;
    }   
    public Skill            GetSkill        (int classID, int skillID)              
    {
        for (int i = 0; i < skills.Count; i++)
        {
            if (skills[i].baseInfo.id == skillID && skills[i].skillClass.baseInfo.id == classID)
            {
                return skills[i];
            }
        }

        Debug.LogError("Could not find the skill with class ID " + classID + " and skill ID " + skillID + ".");
        return null;
    }
    public Skill            GetSkill        (string className, string skillName)    
    {
        for (int i = 0; i < skills.Count; i++)
        {
            if (skills[i].baseInfo.name == skillName && skills[i].skillClass.baseInfo.name == className)
            {
                return skills[i];
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
}
