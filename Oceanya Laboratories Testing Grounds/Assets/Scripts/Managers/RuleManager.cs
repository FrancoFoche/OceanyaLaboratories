using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team                
{
    Enemy,
    Ally,
    OutOfCombat
}
public enum SkillResources      
{
    NatureEnergy,
    Mana,
    Bloodstacks,
    Puppets,
    other
}
public enum SkillType           
{
    Passive,
    Active
}
public enum Stats               
{
    CURHP,
    MAXHP,
    STR,
    INT,
    CHR,
    AGI,
    MR,
    PR,
    CON,
    HPREGEN
}
public enum TargetType          
{
    Self,
    Single,
    Multiple,
    AllAllies,
    AllEnemies,
    Bounce
}
public enum DamageType          
{
    Direct,
    Magical,
    Physical
}
public enum ElementType         
{
    Normal,
    Water,
    Fire,
    Thunder,
    Ice,
    Wind,
    Holy,
    Dark
}
public enum CDType              
{
    Turns,
    Other
}
public enum CooldownStates
{
    BeingUsed,
    Usable,
    OnCooldown,
    Used
}
public enum ActivationTime   
{
    StartOfBattle,
    StartOfTurn,
    WhenTargetting,
    WhenAttacked,
    EndOfTurn,
    Once
}
public enum ClassNames          
{
    Nobody,
    SenjutsuMastery,
    DojutsuMastery,
    FrostGiant,
    MartialArtist,
    Assassin,
    MasterOfDarkArts,
    Gunslinger,
    Technician,
    Vampire,
    Doctor,
    PuppetMaster,
    Ninja,
    MonsterHunter
}
public enum SenjutsuSubclasses  
{
    WoodStyleSage,
    FrogStyleSage,
    SerpentStyleSage
}
public enum operationActions    
{
    Multiply,
    Divide,
    ToThePowerOf
}
public enum CharActions         
{
    Attack,
    Defend,
    Skill,
    Item,
    Rearrange,
    Prepare,
    Skip
}
public enum ReplaceStringVariables
{
    _damage_,
    _heal_,
    _statsModified_,
    _resourcesModified_,
    _shield_,
    _caster_,
    _target_
}

/// <summary>
/// Where enums are stored, and where helpers are created for iteration through them
/// </summary>
public static class RuleManager
{
    /// <summary>
    /// An array that you can use in For functions to iterate through the stats.
    /// </summary>
    public static Stats[] StatHelper { get; private set; }

    /// <summary>
    /// An array that you can use in For functions to iterate through the Skill Resources.
    /// </summary>
    public static SkillResources[] SkillResourceHelper { get; private set; }

    /// <summary>
    /// An array that you can use in For functions to iterate through the Skill Resources.
    /// </summary>
    public static CharActions[] CharActionsHelper { get; private set; }

    /// <summary>
    /// An array that you can use in For functions to iterate through the replaceable strings.
    /// </summary>
    public static ReplaceStringVariables[] ReplaceableStringsHelper { get; private set; }

    public static void BuildHelpers()
    {
        StatHelper = (Stats[])Enum.GetValues(typeof(Stats));

        SkillResourceHelper = (SkillResources[])Enum.GetValues(typeof(SkillResources));

        CharActionsHelper = (CharActions[])Enum.GetValues(typeof(CharActions));

        ReplaceableStringsHelper = (ReplaceStringVariables[])Enum.GetValues(typeof(ReplaceStringVariables));
    }
}
