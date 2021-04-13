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

public enum SkillType
{
    Active,
    Passive
}
public enum TargetType
{
    Self,
    Single,
    MultiTarget,
    AllAllies,
    AllEnemies
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
    OnceABattle,
    OnceADay
}


/// <summary>
/// Where most enums are made, and where helpers are created for iteration through them
/// </summary>
public static class RuleManager
{
    /// <summary>
    /// A dictionary that you can use in For functions to iterate through the stats.
    /// </summary>
    public static Dictionary<int, Stats> StatHelper { get; set; }

    /// <summary>
    /// A dictionary that you can use in For functions to iterate through the Skill Resources.
    /// </summary>
    public static Dictionary<int, SkillResources> SkillResourceHelper { get; set; }

    public static void BuildHelpers()
    {
        StatHelper = new Dictionary<int, Stats>
        {
            { 0, Stats.CURHP },
            { 1, Stats.MAXHP },
            { 2, Stats.STR },
            { 3, Stats.INT },
            { 4, Stats.CHR },
            { 5, Stats.AGI },
            { 6, Stats.MR },
            { 7, Stats.PR },
            { 8, Stats.CON },
            { 9, Stats.HPREGEN }
        };

        SkillResourceHelper = new Dictionary<int, SkillResources>
        {
            { 0, SkillResources.NatureEnergy },
            { 1, SkillResources.Mana },
            { 2, SkillResources.Bloodstacks },
            { 3, SkillResources.Puppets },
            { 4, SkillResources.other },
        };
    }
}
