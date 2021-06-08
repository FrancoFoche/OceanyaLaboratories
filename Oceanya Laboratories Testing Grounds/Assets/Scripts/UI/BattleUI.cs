using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [Header("Character Loaded")]
    [SerializeField] protected int charHashCode;
    [SerializeField] protected CharacterType type;
    [SerializeField] protected int charID;
    [SerializeField] protected string charName;
    [SerializeField] protected BattleUI curUI;
    [SerializeField] protected int charHP;
    [SerializeField] protected int charSTR;
    [SerializeField] protected Character _loadedChar;
    public Character loadedChar { get { return _loadedChar; } private set { _loadedChar = value; } }

    [Header("BASE INFO")]
    public Text nameText;

    public Slider hpSlider;
    public Text hpText;

    public Text statusEffectText;

    public CharacterStatToolTip toolTip;
    public EffectAnimator effectAnimator;
    public Image targettingModeImage;
    public Image raycastBlock;

    private void Update()
    {
        if(loadedChar != null)
        {
            #region Inspector Update, just for testing.
            charName = loadedChar.name;
            charID = loadedChar.ID;
            curUI = loadedChar.curUI;
            charHP = loadedChar.stats[Stats.CURHP];
            charSTR = loadedChar.stats[Stats.STR];
            charHashCode = loadedChar.GetHashCode();
            #endregion
        }
    }

    public virtual void LoadChar(Character character)
    {
        loadedChar = character;
        charID = character.ID;
        nameText.text = character.name;
        hpSlider.minValue = 0;
        hpSlider.maxValue = character.stats[Stats.MAXHP];
        hpSlider.value = character.stats[Stats.CURHP];

        hpText.text = character.stats[Stats.CURHP] + " / " + character.stats[Stats.MAXHP];

        statusEffectText.text = "None";

        toolTip.LoadCharStats(character);
    }

    public virtual void UpdateUI()
    {
        
    }

    public void TargettingMode(bool state)
    {
        targettingModeImage.gameObject.SetActive(state);
    }

    public void InteractableUI(bool state)
    {
        raycastBlock.gameObject.SetActive(!state);
    }
}
