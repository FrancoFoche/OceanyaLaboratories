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

                _instance.RefreshSkillDatabase();
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
        if (BattleManager.instance.scriptableObjectMode)
        {
            for (int i = 0; i < playerCharacters.Length; i++)
            {
                if (playerCharacters[i].ID == id)
                {
                    return playerCharacters[i];
                }
            }
        }
        else
        {
            for (int i = 0; i < DBPlayerCharacter.pCharacters.Count; i++)
            {
                PlayerCharacter current = DBPlayerCharacter.pCharacters[i];
                if (current.ID == id)
                {
                    return current;
                }
            }
        }

        Debug.LogError("Could not find the Player Character with id " + id + ".");
        return null;
    }
    public PlayerCharacter  GetPC           (string name)                           
    {
        if (BattleManager.instance.scriptableObjectMode)
        {
            for (int i = 0; i < playerCharacters.Length; i++)
            {
                if (playerCharacters[i].name == name)
                {
                    return playerCharacters[i];
                }
            }
        }
        else
        {
            for (int i = 0; i < DBPlayerCharacter.pCharacters.Count; i++)
            {
                PlayerCharacter current = DBPlayerCharacter.pCharacters[i];
                if (current.name == name)
                {
                    return current;
                }
            }
        }

        Debug.LogError("Could not find the Player Character with name " + name + ".");
        return null;
    }
    public Enemy            GetEnemy        (int id)                                
    {
        if (BattleManager.instance.scriptableObjectMode)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i].ID == id)
                {
                    return enemies[i];
                }
            }
        }
        else
        {
            for (int i = 0; i < DBEnemies.enemies.Count; i++)
            {
                Enemy current = DBEnemies.enemies[i];
                if (current.ID == id)
                {
                    return current;
                }
            }
        }

        Debug.LogError("Could not find the enemy with id " + id + ".");
        return null;
    }
    public Enemy            GetEnemy        (string name)                           
    {
        if (BattleManager.instance.scriptableObjectMode)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i].name == name)
                {
                    return enemies[i];
                }
            }
        }
        else
        {
            for (int i = 0; i < DBEnemies.enemies.Count; i++)
            {
                Enemy current = DBEnemies.enemies[i];
                if (current.name == name)
                {
                    return current;
                }
            }
        }

        Debug.LogError("Could not find the enemy with name " + name + ".");
        return null;
    }
    public Item             GetItem         (int id)                                
    {
        if (BattleManager.instance.scriptableObjectMode)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                {
                    if (items[i].ID == id)
                    {
                        return items[i];
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < DBItems.items.Count; i++)
            {
                Item current = DBItems.items[i];
                if (current.ID == id)
                {
                    return current;
                }
            }
        }

        Debug.LogError("Could not find the enemy with id " + id + ".");
        return null;
    }
    public Item             GetItem         (string name)                           
    {
        if (BattleManager.instance.scriptableObjectMode)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                {
                    if (items[i].name == name)
                    {
                        return items[i];
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < DBItems.items.Count; i++)
            {
                Item current = DBItems.items[i];
                if (current.name == name)
                {
                    return current;
                }
            }
        }

        Debug.LogError("Could not find the enemy with name " + name + ".");
        return null;
    }
    public BaseSkillClass   GetClass        (int id)                                
    {
        if (BattleManager.instance.scriptableObjectMode)
        {
            for (int i = 0; i < classes.Length; i++)
            {
                if (classes[i] != null)
                {
                    if (classes[i].ID == id)
                    {
                        return classes[i];
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < DBClasses.classes.Count; i++)
            {
                BaseSkillClass current = DBClasses.classes[i];
                if (current.ID == id)
                {
                    return current;
                }
            }
        }
        

        Debug.LogError("Could not find the class with id " + id + ".");
        return null;
    }
    public BaseSkillClass   GetClass        (string name)                           
    {
        if (BattleManager.instance.scriptableObjectMode)
        {
            for (int i = 0; i < classes.Length; i++)
            {
                if (classes[i] != null)
                {
                    if (classes[i].name == name)
                    {
                        return classes[i];
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < DBClasses.classes.Count; i++)
            {
                BaseSkillClass current = DBClasses.classes[i];
                if (current.name == name)
                {
                    return current;
                }
            }
        }

        Debug.LogError("Could not find the class with name " + name + ".");
        return null;
    }   
    public Skill            GetSkill        (int classID, int skillID)              
    {
        if (BattleManager.instance.scriptableObjectMode)
        {
            for (int i = 0; i < skills.Count; i++)
            {
                if (skills[i].ID == skillID && skills[i].skillClass.ID == classID)
                {
                    return skills[i];
                }
            }
        }
        else
        {
            for (int i = 0; i < DBSkills.skills.Count; i++)
            {
                Skill current = DBSkills.skills[i];
                if (current.skillClass.ID == classID && current.ID == skillID)
                {
                    return current;
                }
            }
        }

        Debug.LogError("Could not find the skill with class ID " + classID + " and skill ID " + skillID + ".");
        return null;
    }
    public Skill            GetSkill        (string className, string skillName)    
    {
        if (BattleManager.instance.scriptableObjectMode)
        {
            for (int i = 0; i < skills.Count; i++)
            {
                if (skills[i].name == skillName && skills[i].skillClass.name == className)
                {
                    return skills[i];
                }
            }
        }
        else
        {
            for (int i = 0; i < DBSkills.skills.Count; i++)
            {
                Skill current = DBSkills.skills[i];
                if (current.skillClass.name == className && current.name == skillName)
                {
                    return current;
                }
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

    public void RefreshSkillDatabase()
    {
        if (_instance.skills == null)
        {
            _instance.skills = new List<Skill>();
        }

        if (_instance.classes != null)
        {
            for (int i = 0; i < _instance.classes.Length; i++)
            {
                BaseSkillClass currentClass = _instance.classes[i];

                if (currentClass != null)
                {
                    for (int j = 0; j < currentClass.skillList.Count; j++)
                    {
                        Skill currentSkill = currentClass.skillList[j];
                        currentSkill.skillClass = currentClass;
                        _instance.skills.Add(currentSkill);
                    }
                }
            }
        }
    }
}
