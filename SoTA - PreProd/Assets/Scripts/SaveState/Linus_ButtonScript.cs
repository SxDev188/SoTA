using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Linus_ButtonScript : Linus_Signaler, IInteractable
{
    [SerializeField] private bool hasTimer = false;
    [SerializeField] private float totalTimerDuration = 3;

    private bool isTimerRunning = false;
    void Start()
    {
        signalerSFX = AudioManager.Instance.CreateInstance(FMODEvents.Instance.ButtonSFX);
    }

    public void Interact()
    {
        if (isPushed && isTimerRunning)
        {
            signalerSFX.setParameterByNameWithLabel("ButtonPushState", "PushFail");
            signalerSFX.start();

            return; //we busy
        }

        if (!isPushed && !hasTimer)
        {
            signalerSFX.setParameterByNameWithLabel("ButtonPushState", "PushDown");

            ActivateAllPuzzleElements();
            isPushed = true;
        }
        else if (!isPushed && hasTimer)
        {
            signalerSFX.setParameterByNameWithLabel("ButtonPushState", "PushDown");

            StartTimerForAllPuzzleElements();
            isPushed = true;
        }
        else if (isPushed && !isTimerRunning)
        {
            signalerSFX.setParameterByNameWithLabel("ButtonPushState", "PushUp");

            DeactivateAllPuzzleElements();
            isPushed = false;
        }

        signalerSFX.start();
    }
}
