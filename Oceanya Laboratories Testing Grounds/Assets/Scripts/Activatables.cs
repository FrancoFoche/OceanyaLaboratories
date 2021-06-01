using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Activatables
{
    [SerializeField] private string                                 _name;
    [SerializeField] private int                                    _ID;
    [SerializeField] private string                                 _description;
    [SerializeField] private ActivatableType                        _activatableType;

    [SerializeField] private List<Behaviors>                        _behaviors                          = new List<Behaviors>();

    [SerializeField] private string                                 _activationText;

    [SerializeField] private bool                                   _hasActivationRequirement;
    [SerializeField] private List<ActivationRequirement>            _activationRequirements             = new List<ActivationRequirement>();

    [SerializeField] private bool                                   _hasPassive;
    [SerializeField] private ActivationTime                         _passiveActivationType;

    [SerializeField] private bool                                   _lasts;
    [SerializeField] private int                                    _lastsFor;

    [SerializeField] private TargetType                             _targetType;
    [SerializeField] private int                                    _maxTargets;

    [SerializeField] private CDType                                 _cdType;
    [SerializeField] private int                                    _cooldown;

    [SerializeField] private bool                                   _doesDamage;
    [SerializeField] private DamageType                             _damageType;
    [SerializeField] private ElementType                            _damageElement;
    [SerializeField] private List<RPGFormula>                       _damageFormula                      = new List<RPGFormula>();

    [SerializeField] private bool                                   _doesHeal;
    [SerializeField] private List<RPGFormula>                       _healFormula                        = new List<RPGFormula>();

    [SerializeField] private bool                                   _flatModifiesStat;
    [SerializeField] private Dictionary<Stats, int>                 _flatStatModifiers                  = new Dictionary<Stats, int>();
    [SerializeField] private bool                                   _formulaModifiesStat;
    [SerializeField] private Dictionary<Stats, List<RPGFormula>>    _formulaStatModifiers               = new Dictionary<Stats, List<RPGFormula>>();
    [SerializeField] private StatModificationTypes                  _modificationType;

    [SerializeField] private bool                                   _modifiesResource;
    [SerializeField] private Dictionary<SkillResources, int>        _resourceModifiers                  = new Dictionary<SkillResources, int>();

    [SerializeField] private bool                                   _unlocksResource;
    [SerializeField] private List<SkillResources>                   _unlockedResources                  = new List<SkillResources>();

    [SerializeField] private bool                                   _changesBasicAttack;
    [SerializeField] private List<RPGFormula>                       _newBasicAttackFormula              = new List<RPGFormula>();
    [SerializeField] private DamageType                             _newBasicAttackDamageType;

    [SerializeField] private bool                                   _revives;

    [SerializeField] private bool                                   _costsTurn;

    [SerializeField] private bool                                   _appliesStatusEffects;

    [SerializeField] private bool                                   _doesSummon;

    [SerializeField] private bool                                   _doesShield;

    #region Getters/Setters
    public string                                   name                                { get { return _name; }                         protected set { _name = value; } }
    public int                                      ID                                  { get { return _ID; }                           protected set { _ID = value; } }
    public string                                   description                         { get { return _description; }                  protected set { _description = value; } }
    public ActivatableType                          activatableType                     { get { return _activatableType; }              protected set { _activatableType = value; } }

    public List<Behaviors>                          behaviors                           { get { return _behaviors; }                    protected set { _behaviors = value; } }

    public string                                   activationText                      { get { return _activationText; }               protected set { _activationText = value; } }

    public bool                                     hasActivationRequirement            { get { return _hasActivationRequirement; }     protected set { _hasActivationRequirement = value; } }
    public List<ActivationRequirement>              activationRequirements              { get { return _activationRequirements; }       protected set { _activationRequirements = value; } }

    public bool                                     hasPassive                          { get { return _hasPassive; }                   protected set { _hasPassive = value; } }
    public ActivationTime                           passiveActivationType               { get { return _passiveActivationType; }        protected set { _passiveActivationType = value; } }

    public bool                                     lasts                               { get { return _lasts; }                        protected set { _lasts = value; } }
    public int                                      lastsFor                            { get { return _lastsFor; }                     protected set { _lastsFor = value; } }

    public TargetType                               targetType                          { get { return _targetType; }                   protected set { _targetType = value; } }
    public int                                      maxTargets                          { get { return _maxTargets; }                   protected set { _maxTargets = value; } }

    public CDType                                   cdType                              { get { return _cdType; }                       protected set { _cdType = value; } }
    public int                                      cooldown                            { get { return _cooldown; }                     protected set { _cooldown = value; } }

    public bool                                     doesDamage                          { get { return _doesDamage; }                   protected set { _doesDamage = value; } }
    public DamageType                               damageType                          { get { return _damageType; }                   protected set { _damageType = value; } }
    public ElementType                              damageElement                       { get { return _damageElement; }                protected set { _damageElement = value; } }
    public List<RPGFormula>                         damageFormula                       { get { return _damageFormula; }                protected set { _damageFormula = value; } }

    public bool                                     doesHeal                            { get { return _doesHeal; }                     protected set { _doesHeal = value; } }
    public List<RPGFormula>                         healFormula                         { get { return _healFormula; }                  protected set { _healFormula = value; } }

    public bool                                     flatModifiesStat                    { get { return _flatModifiesStat; }             protected set { _flatModifiesStat = value; } }
    public Dictionary<Stats, int>                   flatStatModifiers                   { get { return _flatStatModifiers; }            protected set { _flatStatModifiers = value; } }
    public bool                                     formulaModifiesStat                 { get { return _formulaModifiesStat; }          protected set { _formulaModifiesStat = value; } }
    public Dictionary<Stats, List<RPGFormula>>      formulaStatModifiers                { get { return _formulaStatModifiers; }         protected set { _formulaStatModifiers = value; } }
    public StatModificationTypes                    modificationType                    { get { return _modificationType; }             protected set { _modificationType = value; } }

    public bool                                     modifiesResource                    { get { return _modifiesResource; }             protected set { _modifiesResource = value; } }
    public Dictionary<SkillResources, int>          resourceModifiers                   { get { return _resourceModifiers; }            protected set { _resourceModifiers = value; } }

    public bool                                     unlocksResource                     { get { return _unlocksResource; }              protected set { _unlocksResource = value; } }
    public List<SkillResources>                     unlockedResources                   { get { return _unlockedResources; }            protected set { _unlockedResources = value; } }

    public bool                                     changesBasicAttack                  { get { return _changesBasicAttack; }           protected set { _changesBasicAttack = value; } }
    public List<RPGFormula>                         newBasicAttackFormula               { get { return _newBasicAttackFormula; }        protected set { _newBasicAttackFormula = value; } }
    public DamageType                               newBasicAttackDamageType            { get { return _newBasicAttackDamageType; }     protected set { _newBasicAttackDamageType = value; } }

    public bool                                     revives                             { get { return _revives; }                      protected set { _revives = value; } }

    public bool                                     costsTurn                           { get { return _costsTurn; }                    protected set { _costsTurn = value; } }

    public bool                                     appliesStatusEffects                { get { return _appliesStatusEffects; }         protected set { _appliesStatusEffects = value; } }

    public bool                                     doesSummon                          { get { return _doesSummon; }                   protected set { _doesSummon = value; } }

    public bool                                     doesShield                          { get { return _doesShield; }                   protected set { _doesShield = value; } }
    #endregion

    public enum Behaviors
    {
        None,
        DoesDamage,
        HasCooldown,
        DoesHeal,
        FlatModifiesStat,
        FormulaModifiesStat,
        ModifiesResource,
        UnlocksResource,
        Passive,
        CostsTurn,
        ActivationRequirement,
        LastsFor,
        ChangesBasicAttack,
        Revives
    }

    public abstract void    Activate                    (Character caster);

    public abstract void    Action                      (Character caster, List<Character> target);

    public string           ReplaceActivationText       (Dictionary<ReplaceStringVariables, string> replaceWith)                                                            
    {
        string resultText = activationText;

        for (int i = 0; i < RuleManager.ReplaceableStringsHelper.Length; i++)
        {
            ReplaceStringVariables curVariable = RuleManager.ReplaceableStringsHelper[i];
            string curVarString = curVariable.ToString();

            bool dictionaryHasVariable = replaceWith.ContainsKey(curVariable);
            bool textHasVariable = resultText.Contains(curVarString);

            if (dictionaryHasVariable && textHasVariable)
            {
                resultText = resultText.Replace(curVarString, replaceWith[curVariable]);
            }
        }

        return resultText;
    }
}

public class ActivatableInfo
{
    public Character    character       { get; protected set; }
    public bool         activatable     { get; protected set; }
    public bool         equipped        { get; protected set; }
    public bool         wasActivated    { get; protected set; }   //If the skill was activated at SOME point.
    public bool         currentlyActive { get; protected set; }   //If the skill is currently active
    public int          activatedAt     { get; protected set; }   //when the skill was activated
    public int          timesActivated  { get; set; }           //how many times the skill was activated
    public bool         showInfo        { get; protected set; }

    public void Equip()
    {
        equipped = true;
    }
    public void Unequip()
    {
        equipped = false;
    }
    public virtual void SetActive()
    {
        currentlyActive = true;
        wasActivated = true;
        activatedAt = character.timesPlayed;
    }
    public virtual void SetDeactivated()
    {
        timesActivated = 0;
        currentlyActive = false;
    }
    public static bool CheckActivatable(Activatables activatable)
    {
        bool activatableBool = true;

        if (activatable.hasActivationRequirement)
        {
            for (int i = 0; i < activatable.activationRequirements.Count; i++)
            {
                if (!activatable.activationRequirements[i].CheckRequirement())
                {
                    activatableBool = false;
                }
            }

        }

        return activatableBool;
    }
}