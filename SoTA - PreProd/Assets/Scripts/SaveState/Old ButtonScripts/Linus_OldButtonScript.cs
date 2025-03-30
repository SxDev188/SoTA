using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Linus_OldButtonScript : Linus_Signaler, IInteractable
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
        //TimerIsOn
        if (isTimerRunning)
        {
            signalerSFX.setParameterByNameWithLabel("ButtonPushState", "PushFail");
            signalerSFX.start();
            return; //Stop Here
        }
        //Timer Exists but is off
        if (hasTimer)
        {
            signalerSFX.setParameterByNameWithLabel("ButtonPushState", "PushDown");
            isPushed = true;
            StartTimerForAllPuzzleElements();
        }
        //No Timer
        else
        {
            if (isPushed)
            {
                signalerSFX.setParameterByNameWithLabel("ButtonPushState", "PushUp");
            }
            else
            {
                signalerSFX.setParameterByNameWithLabel("ButtonPushState", "PushDown");
            }
            isPushed = !isPushed;
            SetElements();
        }
        signalerSFX.start();
    }
    private void StartTimerForAllPuzzleElements()
    {
        foreach (GameObject puzzleElement in puzzleElements)
        {
            IActivatable activatable = puzzleElement.GetComponent<IActivatable>();

            if (activatable == null)
            {
                continue;
            }

            activatable.Activate();
            StartCoroutine(DeactivateDelayed(activatable));
            isTimerRunning = true;
        }
    }

    private IEnumerator DeactivateDelayed(IActivatable activatable)
    {
        yield return new WaitForSeconds(totalTimerDuration);
        activatable.Deactivate();
        isPushed = false;
        isTimerRunning = false;

        signalerSFX.setParameterByNameWithLabel("ButtonPushState", "PushUp");
        signalerSFX.start();
    }
}
