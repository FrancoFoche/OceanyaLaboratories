using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// just a testing class, nothing here makes sense so don't worry about it
/// </summary>
public class TestingClass : MonoBehaviour
{
    public AllyBattleUI test;
    public Toggle toggle;
    private bool interactable;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            interactable = !interactable;
            test.InteractableUI(interactable);
            Debug.Log("Interactable = " + interactable);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            toggle.isOn = !toggle.isOn;
            Debug.Log("IsOn = " + toggle.isOn);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            //toggle.isOn = !toggle.isOn;
            //toggle.isOn = !toggle.isOn;

            
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Success");
        }
    }


}
