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
        throw new System.NotImplementedException();
    }
}
