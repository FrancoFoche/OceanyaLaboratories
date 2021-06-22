using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenu_TeamOrder : ObjectList
{
    [Header("Settings")]
    public int turnsBeforeScroll = 4;
    public int turnsAfterScroll = 5;
    public float alphaOfNonTurns = 0.1f;
    public float alphaOfCurrentTurns = 0.5f;
    public float canvasAlphaPastTurns = 0.5f;
    public float canvasAlphaDead = 0.25f;

    [Header("Colors")]
    public Color enemyTurnColor;
    public Color allyTurnColor;

    [Header("Assignables")]
    public Animator animator;
    public RectTransform contentPanel;
    public ScrollRect scroll;
    public Toggle dropdownToggle;

    private List<Character> teamOrder;

    public void LoadTeamOrder(List<Character> teamOrder, int currentTurnIndex)
    {
        ClearList();
        this.teamOrder = teamOrder;

        bool afterCurrentTurn = false;
        for (int i = 0; i < teamOrder.Count; i++)
        {
            Character currentCharacter = teamOrder[i];

            #region Add object and scroll to the right turn
            GameObject newObj = AddObject();
            TeamOrderPosition pos = newObj.GetComponent<TeamOrderPosition>();
            bool isCurrentTurn = false;
            
            if (currentTurnIndex == i)
            {
                isCurrentTurn = true;
                afterCurrentTurn = true;
                if(i < teamOrder.Count - turnsAfterScroll)
                {
                    if (i >= turnsBeforeScroll)
                    {
                        ScrollTo(pos.GetComponent<RectTransform>(), i, turnsBeforeScroll);
                    }
                    else
                    {
                        contentPanel.anchoredPosition = new Vector2(contentPanel.anchoredPosition.x, 0);
                    }
                }
            }
            #endregion

            #region Settings
            Color backgroundColor;
            float alpha = alphaOfNonTurns;
            float canvasOpacity = canvasAlphaPastTurns;

            if (afterCurrentTurn)
            {
                canvasOpacity = 1f;
            }

            if (currentCharacter.dead)
            {
                canvasOpacity = canvasAlphaDead;
            }

            #region Color
            if (currentCharacter.team == Team.Ally)
            {
                backgroundColor = allyTurnColor;
            }
            else
            {
                backgroundColor = enemyTurnColor;
            }
            #endregion

            if (isCurrentTurn)
            {
                alpha = alphaOfCurrentTurns;
            }
            #endregion

            pos.LoadCharacter(teamOrder[i], i + 1, backgroundColor, alpha, canvasOpacity);
        }
    }

    public void UpdateTeamOrder(int currentTurnIndex)
    {
        LoadTeamOrder(teamOrder, currentTurnIndex);
    }

    public void CheckOpen(Toggle toggle)
    {
        if (toggle.isOn)
        {
            animator.SetTrigger("Open");
        }
        else
        {
            animator.SetTrigger("Close");
        }
    }

    public void ScrollTo(RectTransform target, int position, int previousTurnLimit)
    {
        Canvas.ForceUpdateCanvases();
        Vector2 newVector = new Vector2(contentPanel.anchoredPosition.x, target.rect.height * position - target.rect.height * previousTurnLimit);

        if(newVector.y < 0) { 
        }
        contentPanel.anchoredPosition = newVector;
    }

    public void ToggleVisibility()
    {
        if (dropdownToggle.isOn)
        {
            dropdownToggle.isOn = false;
        }
        else
        {
            dropdownToggle.isOn = true;
        }
    }
}