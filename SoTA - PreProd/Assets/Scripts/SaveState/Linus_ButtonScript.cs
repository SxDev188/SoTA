using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class Linus_ButtonScript : MonoBehaviour, IInteractable
{
    [SerializeField] private List<GameObject> puzzleElements = new List<GameObject>();
    [SerializeField] private bool hasTimer = false;
    [SerializeField] private float totalTimerDuration = 3;

    private bool isPushed = false;
    private bool isTimerRunning = false;
    private Transform player;

    private EventInstance buttonSFX;
    public bool IsActive { get { return isPushed; } }

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        buttonSFX = AudioManager.Instance.CreateInstance(FMODEvents.Instance.ButtonSFX);

    }

    public void Interact()
    {
        if (isPushed && isTimerRunning)
        {
            buttonSFX.setParameterByNameWithLabel("ButtonPushState", "PushFail");
            buttonSFX.start();

            return; //we busy
        }

        if (!isPushed && !hasTimer)
        {
            buttonSFX.setParameterByNameWithLabel("ButtonPushState", "PushDown");

            ActivateAllPuzzleElements();
            isPushed = true;
        }
        else if (!isPushed && hasTimer)
        {
            buttonSFX.setParameterByNameWithLabel("ButtonPushState", "PushDown");

            StartTimerForAllPuzzleElements();
            isPushed = true;
        }
        else if (isPushed && !isTimerRunning)
        {
            buttonSFX.setParameterByNameWithLabel("ButtonPushState", "PushUp");

            DeactivateAllPuzzleElements();
            isPushed = false;
        }

        buttonSFX.start();
    }

    private void ActivateAllPuzzleElements()
    {
        foreach (GameObject puzzleElement in puzzleElements)
        {
            IActivatable activatable = puzzleElement.GetComponent<IActivatable>();

            if (activatable == null)
            {
                continue;
            }

            activatable.Activate();
        }
    }
    private void DeactivateAllPuzzleElements()
    {
        foreach (GameObject puzzleElement in puzzleElements)
        {
            IActivatable activatable = puzzleElement.GetComponent<IActivatable>();

            if (activatable == null)
            {
                continue;
            }

            activatable.Deactivate();
        }
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
        if (isTimerRunning)
        {
            activatable.Deactivate();
            isPushed = false;
            isTimerRunning = false;

            buttonSFX.setParameterByNameWithLabel("ButtonPushState", "PushUp");
            buttonSFX.start();
        }
    }
    public void SetState(bool Active)
    {
        if (hasTimer)
        {
            if (isTimerRunning)
            {
                DeactivateAllPuzzleElements();
                isTimerRunning = false;
                isPushed = false;
            }
            return;
        }
        isPushed = Active;
        if (Active)
        {
            ActivateAllPuzzleElements();
        }
        else
        {
            DeactivateAllPuzzleElements();
        }
    }
    private void ToggleButtonState()
    {
        isPushed = !isPushed;
    }
}
