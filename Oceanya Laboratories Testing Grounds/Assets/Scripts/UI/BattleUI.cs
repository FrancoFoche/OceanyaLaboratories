using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [Header("Character Loaded")]
    public CharacterType type;
    public int charID;
    public string charName;

    public Character loadedChar { get; private set; }

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
        if (loadedChar != null)
        {
            charName = loadedChar.name;
        }
        else
        {
            charName = "Char does not exist";
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
