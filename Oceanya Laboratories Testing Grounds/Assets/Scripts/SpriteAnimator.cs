using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    public enum Animations
    {
        Attacked,
        Death
    }

    public Animator animator;

    private void Update()
    {
        Testing();
    }

    public void PlayAnimation(Animations animations)
    {
        Debug.Log("Played animation: " + animations.ToString() + ".");
        animator.SetTrigger(animations.ToString());
    }

    public void PlaySound(Sounds sound)
    {
        SFXManager.PlaySound(sound);
    }
    /// <summary>
    /// Just a testing function that reads your input
    /// </summary>
    public void Testing()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            PlayAnimation(Animations.Attacked);
        }
    }
}
