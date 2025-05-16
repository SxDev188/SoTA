using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueInstructionText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI keyboard;
    [SerializeField] TextMeshProUGUI controller;

    void Update()
    {
        if (UIScript.IsUsingController)
        {
            if(!controller.gameObject.activeInHierarchy) //we are using controller but the controller text is diabled
            {
                controller.gameObject.SetActive(true);
                keyboard.gameObject.SetActive(false);
            }
        } else
        {
            if (!keyboard.gameObject.activeInHierarchy) //we are using keyboard but the keyboard text is diabled
            {
                keyboard.gameObject.SetActive(true);
                controller.gameObject.SetActive(false);
            }
        }
    }
}
