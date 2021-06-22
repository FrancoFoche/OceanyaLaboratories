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


    [SerializeField] private List<ActivationRequirement>            _activationRequirements             = new List<ActivationRequirement>();


    [SerializeField] private ActivationTime                         _passiveActivationType;


    [SerializeField] private int                                    _lastsFor;

    [SerializeField] private TargetType                             _targetType;
    [SerializeField] private int                                    _maxTargets;

    [SerializeField] private CDType                                 _cdType;
    [SerializeField] private int                                    _cooldown;


    [SerializeField] private DamageType                             _damageType;
    [SerializeField] private ElementType                            _damageElement;
    [SerializeField] private List<RPGFormula>                       _damageFormula                      = new List<RPGFormula>();
    [SerializeField] private int                                    _damageFlat;

    [SerializeField] private List<RPGFormula>                       _healFormula                        = new List<RPGFormula>();
    [SerializeField] private int                                    _healFlat;


    [SerializeField] private Dictionary<Stats, int>                 _flatStatModifiers                  = new Dictionary<Stats, int>();

    [SerializeField] private Dictionary<Stats, List<RPGFormula>>    _formulaStatModifiers               = new Dictionary<Stats, List<RPGFormula>>();
    [SerializeField] private StatModificationTypes                  _modificationType;


    [SerializeField] private Dictionary<SkillResources, int>        _resourceModifiers                  = new Dictionary<SkillResources, int>();

    [SerializeField] private bool                                   _unlocksResource;
    [SerializeField] private List<SkillResources>                   _unlockedResources                  = new List<SkillResources>();


    [SerializeField] private List<RPGFormula>                       _newBasicAttackFormula              = new List<RPGFormula>();
    [SerializeField] private DamageType                             _newBasicAttackDamageType;

    #region Getters/Setters
    public string                                   name                                { get { return _name; }                         protected set { _name = value; } }
    public int                                      ID                                  { get { return _ID; }                           protected set { _ID = value; } }
    public string                                   description                         { get { return _description; }                  protected set { _description = value; } }
    public ActivatableType                          activatableType                     { get { return _activatableType; }              protected set { _activatableType = value; } }

    public List<Behaviors>                          behaviors                           { get { return _behaviors; }                    protected set { _behaviors = value; } }

    public string                                   activationText                      { get { return _activationText; }               protected set { _activationText = value; } }

    public List<ActivationRequirement>              activationRequirements              { get { return _activationRequirements; }       protected set { _activationRequirements = value; } }

    public ActivationTime                           passiveActivationType               { get { return _passiveActivationType; }        protected set { _passiveActivationType = value; } }

    public int                                      lastsFor                            { get { return _lastsFor; }                     protected set { _lastsFor = value; } }

    public TargetType                               targetType                          { get { return _targetType; }                   protected set { _targetType = value; } }
    public int                                      maxTargets                          { get { return _maxTargets; }                   protected set { _maxTargets = value; } }

    public CDType                                   cdType                              { get { return _cdType; }                       protected set { _cdType = value; } }
    public int                                      cooldown                            { get { return _cooldown; }                     protected set { _cooldown = value; } }

    public DamageType                               damageType                          { get { return _damageType; }                   protected set { _damageType = value; } }
    public ElementType                              damageElement                       { get { return _damageElement; }                protected set { _damageElement = value; } }
    public List<RPGFormula>                         damageFormula                       { get { return _damageFormula; }                protected set { _damageFormula = value; } }
    public int                                      damageFlat                          { get { return _damageFlat; }                   protected set { _damageFlat = value; } }


    public List<RPGFormula>                         healFormula                         { get { return _healFormula; }                  protected set { _healFormula = value; } }
    public int                                      healFlat                            { get { return _healFlat; }                     protected set { _healFlat = value; } }

    public Dictionary<Stats, int>                   flatStatModifiers                   { get { return _flatStatModifiers; }            protected set { _flatStatModifiers = value; } }
    public Dictionary<Stats, List<RPGFormula>>      formulaStatModifiers                { get { return _formulaStatModifiers; }         protected set { _formulaStatModifiers = value; } }
    public StatModificationTypes                    modificationType                    { get { return _modificationType; }             protected set { _modificationType = value; } }

    public Dictionary<SkillResources, int>          resourceModifiers                   { get { return _resourceModifiers; }            protected set { _resourceModifiers = value; } }

    public List<SkillResources>                     unlockedResources                   { get { return _unlockedResources; }            protected set { _unlockedResources = value; } }

    public List<RPGFormula>                         newBasicAttackFormula               { get { return _newBasicAttackFormula; }        protected set { _newBasicAttackFormula = value; } }
    public DamageType                               newBasicAttackDamageType            { get { return _newBasicAttackDamageType; }     protected set { _newBasicAttackDamageType = value; } }

    #endregion

    public enum Behaviors
    {
        None,
        CustomAnimation,
        DoesDamage_Formula,
        DoesDamage_Flat,
        HasCooldown,
        DoesHeal_Formula,
        DoesHeal_Flat,
        ModifiesStat_Flat,
        ModifiesStat_Formula,
        ModifiesResource,
        UnlocksResource,
        Passive,
        CostsTurn,
        ActivationRequirement,
        LastsFor,
        ChangesBasicAttack,
        Revives,
        AppliesStatusEffects,
        DoesSummon,
        DoesShield
    }

    public void             Activate                    (Character caster, ActivatableInfo info)                                                                            
    {
        info.UpdateActivatable(this);

        if (info.activatable)
        {
            bool firstActivation = !info.currentlyActive;

            if (activatableType == ActivatableType.Active && firstActivation && behaviors.Contains(Behaviors.Passive))
            {
                BattleManager.i.battleLog.LogBattleEffect($"The passive of {name} was activated for {caster.name}.");
                info.SetActive();

                if (behaviors.Contains(Behaviors.CostsTurn))
                {
                    TeamOrderManager.EndTurn();
                }
            }
            else
            {
                if (targetType == TargetType.Single || targetType == TargetType.Multiple)
                {
                    UICharacterActions.instance.maxTargets = maxTargets;
                    info.SetAction();
                }
                else
                {
                    List<Character> targets = new List<Character>();
                    switch (targetType)
                    {
                        case TargetType.Self:
                            targets = new List<Character>() { caster };
                            break;

                        case TargetType.AllAllies:
                            if (caster.team == Team.Ally)
                            {
                                targets = TeamOrderManager.allySide;
                            }
                            else
                            {
                                targets = TeamOrderManager.enemySide;
                            }
                            break;
                        case TargetType.AllEnemies:
                            if (caster.team == Team.Ally)
                            {
                                targets = TeamOrderManager.enemySide;
                            }
                            else
                            {
                                targets = TeamOrderManager.allySide;
                            }
                            break;
                        case TargetType.Bounce:
                            targets = new List<Character>() { BattleManager.caster };
                            break;
                    }

                    BattleManager.i.SetTargets(targets);

                    Action(caster, targets, info);
                }
            }
        }
        else
        {
            BattleManager.i.battleLog.LogBattleEffect($"But {caster.name} did not meet the requirements to activate the skill!");
        }
    }

    public void             Action                      (Character caster, List<Character> target, ActivatableInfo info)                                                    
    {
        bool firstActivation = !info.currentlyActive;

        if ((targetType == TargetType.Single || targetType == TargetType.Multiple) && firstActivation)
        {
            info.SetActive();
        }

        for (int i = 0; i < target.Count; i++)
        {
            System.Action turnActions = delegate { };

            Dictionary<ReplaceStringVariables, string> activationText = new Dictionary<ReplaceStringVariables, string>();


            activationText.Add(ReplaceStringVariables._caster_, caster.name);
            activationText.Add(ReplaceStringVariables._target_, target[i].name);

            if (target[i].targettable)
            {
                if (behaviors.Contains(Behaviors.DoesDamage_Flat))
                {
                    int rawDMG = damageFlat;

                    int finalDMG = target[i].CalculateDefenses(rawDMG, damageType);

                    activationText.Add(ReplaceStringVariables._damage_, finalDMG.ToString());

                    turnActions += delegate { target[i].GetsDamagedBy(finalDMG, damageType, caster); };
                }
                if (behaviors.Contains(Behaviors.DoesDamage_Formula))
                {
                    int rawDMG = RPGFormula.ReadAndSumList(damageFormula, caster.stats);

                    int finalDMG = target[i].CalculateDefenses(rawDMG, damageType);

                    activationText.Add(ReplaceStringVariables._damage_, finalDMG.ToString());

                    turnActions += delegate { target[i].GetsDamagedBy(finalDMG, damageType, caster); };
                }
                if (behaviors.Contains(Behaviors.DoesHeal_Flat))
                {
                    int healAmount = healFlat;

                    activationText.Add(ReplaceStringVariables._heal_, healAmount.ToString());

                    turnActions += delegate { target[i].GetsHealedBy(healAmount); };
                }
                if (behaviors.Contains(Behaviors.DoesHeal_Formula))
                {
                    int healAmount = RPGFormula.ReadAndSumList(healFormula, caster.stats);

                    activationText.Add(ReplaceStringVariables._heal_, healAmount.ToString());

                    turnActions += delegate { target[i].GetsHealedBy(healAmount); };
                }
                if (behaviors.Contains(Behaviors.ModifiesStat_Flat))
                {
                    turnActions += delegate { target[i].ModifyStat(modificationType, flatStatModifiers); };
                }
                if (behaviors.Contains(Behaviors.ModifiesStat_Formula))
                {
                    Dictionary<Stats, int> resultModifiers = new Dictionary<Stats, int>();
                    for (int j = 0; j < RuleManager.StatHelper.Length; j++)
                    {
                        Stats currentStat = RuleManager.StatHelper[j];

                        if (formulaStatModifiers.ContainsKey(currentStat))
                        {
                            resultModifiers.Add(currentStat, RPGFormula.ReadAndSumList(formulaStatModifiers[currentStat], caster.stats));
                        }
                    }

                    turnActions += delegate { target[i].ModifyStat(modificationType, resultModifiers); };
                }
                if (behaviors.Contains(Behaviors.UnlocksResource))
                {
                    turnActions += delegate { target[i].UnlockResources(unlockedResources); };
                }
                if (behaviors.Contains(Behaviors.ModifiesResource))
                {
                    turnActions += delegate { target[i].ModifyResource(resourceModifiers); };
                }
                if (behaviors.Contains(Behaviors.ChangesBasicAttack))
                {
                    turnActions += delegate { target[i].ChangeBaseAttack(newBasicAttackFormula, newBasicAttackDamageType); };
                }
                
                if (behaviors.Contains(Behaviors.Revives))
                {
                    turnActions += delegate
                    {
                        if (target[i].dead)
                        {
                            target[i].Revive();
                        }
                        else
                        {
                            BattleManager.i.battleLog.LogBattleEffect($"But {target[i].name} was not dead...");
                        }
                    };
                }

                if (behaviors.Contains(Behaviors.AppliesStatusEffects))
                {

                }

                if (behaviors.Contains(Behaviors.DoesSummon))
                {

                }

                if (behaviors.Contains(Behaviors.DoesShield))
                {

                }
            }
            else
            {
                turnActions += delegate { BattleManager.i.battleLog.LogBattleEffect("Target wasn't targettable, smh"); };
            }

            BattleManager.i.battleLog.LogBattleEffect(ReplaceActivationText(activationText));

            turnActions();
        }


        if (activatableType != ActivatableType.Passive && !behaviors.Contains(Behaviors.Passive))
        {
            info.SetDeactivated();
        }

        if (behaviors.Contains(Behaviors.CostsTurn))
        {
            if (!(activatableType == ActivatableType.Active && behaviors.Contains(Behaviors.Passive) && info.currentlyActive))
            {
                TeamOrderManager.EndTurn();
            }
        }
    }

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

public abstract class ActivatableInfo
{
    public Character    character       { get; protected set; }
    public bool         activatable     { get; protected set; }
    public bool         equipped        { get; protected set; }
    public bool         wasActivated    { get; protected set; }   //If the skill was activated at SOME point.
    public bool         currentlyActive { get; protected set; }   //If the skill is currently active
    public int          activatedAt     { get; protected set; }   //when the skill was activated
    public int          timesActivated  { get; set; }           //how many times the skill was activated
    public bool         showInfo        { get; protected set; }

    public void         Equip               ()                          
    {
        equipped = true;
    }
    public void         Unequip             ()                          
    {
        equipped = false;
    }
    public virtual void SetActive           ()                          
    {
        currentlyActive = true;
        wasActivated = true;
        activatedAt = character.timesPlayed;
    }
    public virtual void SetDeactivated      ()                          
    {
        timesActivated = 0;
        currentlyActive = false;
    }
    public static bool  CheckActivatable    (Activatables activatable)  
    {
        bool activatableBool = true;

        if (activatable.behaviors.Contains(Activatables.Behaviors.ActivationRequirement))
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
    public void         UpdateActivatable   (Activatables activatable)  
    {
        this.activatable = CheckActivatable(activatable);
    }
    public abstract void SetAction();
}