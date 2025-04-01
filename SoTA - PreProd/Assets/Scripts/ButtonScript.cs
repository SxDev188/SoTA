using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class ButtonScript : MonoBehaviour, IInteractable
{
    [SerializeField] private List<GameObject> puzzleElements = new List<GameObject>();
    [SerializeField] private bool hasTimer = false;
    [SerializeField] private float totalTimerDuration = 3;

    private bool isPushed = false;
    private bool isTimerRunning = false;
    private Transform player;
    private Transform button;

    private EventInstance buttonSFX;
    private EventInstance timerTickingSFX;


    public bool IsActive { get { return isPushed; } }

    private Transform FindChildByName(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform found = FindChildByName(child, name);
            if (found != null)
                return found;
        }
        return null;
    }

    public void Start()
    {
        button = FindChildByName(transform, "Button_connection");

        if (button == null)
        {
            Debug.LogError("Button_connection child not found! Check the hierarchy.");
        }

        player = GameObject.FindGameObjectWithTag("Player").transform;
        buttonSFX = AudioManager.Instance.CreateInstance(FMODEvents.Instance.ButtonSFX);
        timerTickingSFX = AudioManager.Instance.CreateInstance(FMODEvents.Instance.TimerTickingSFX);

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
            FlipButtonDown();
        }
        else if (!isPushed && hasTimer)
        {
            buttonSFX.setParameterByNameWithLabel("ButtonPushState", "PushDown");
            timerTickingSFX.start();

            StartTimerForAllPuzzleElements();
            isPushed = true;
            FlipButtonDown();
        }
        else if (isPushed && !isTimerRunning)
        {
            buttonSFX.setParameterByNameWithLabel("ButtonPushState", "PushUp");

            DeactivateAllPuzzleElements();
            isPushed = false;
            FlipButtonUp();
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
        if (isTimerRunning)
        {
            return;
        }

        isTimerRunning = true;
        
        foreach (GameObject puzzleElement in puzzleElements)
        {
            IActivatable activatable = puzzleElement.GetComponent<IActivatable>();

            if (activatable == null)
            {
                continue;
            }

            activatable.Activate();
        }
        
        StartCoroutine(DeactivateAllDelayed());
    }

    private IEnumerator DeactivateAllDelayed()
    {
        yield return new WaitForSeconds(totalTimerDuration);

        foreach (GameObject puzzleElement in puzzleElements)
        {
            IActivatable activatable = puzzleElement.GetComponent<IActivatable>();

            if (activatable == null)
            {
                continue;
            }

            activatable.Deactivate();
        }

        isPushed = false;
        isTimerRunning = false;

        FlipButtonUp();

        timerTickingSFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        buttonSFX.setParameterByNameWithLabel("ButtonPushState", "PushUp");
        buttonSFX.start();
        //if (isTimerRunning)
        //{
        //    activatable.Deactivate();
        //    isPushed = false;
        //    isTimerRunning = false;

        //    FlipButtonUp();

        //    timerTickingSFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        //    buttonSFX.setParameterByNameWithLabel("ButtonPushState", "PushUp");
        //    buttonSFX.start();
        //}
    }
    private void ToggleButtonState()
    {
        isPushed = !isPushed;
    }

    private void FlipButtonDown()
    {
        button.localRotation = Quaternion.Euler(180, 0, 0);
    }

    private void FlipButtonUp()
    {
        button.localRotation = Quaternion.Euler(0, 0, 0);
    }
    public void SetState(bool Active)
    {
        if (hasTimer)
        {
            if (isTimerRunning)
            {
                DeactivateAllPuzzleElements();
                FlipButtonUp();
                isTimerRunning = false;
                isPushed = false;
                timerTickingSFX.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            }
            return;
        }
        if (Active != isPushed)
        {
            isPushed = Active;
            if (Active)
            {
                FlipButtonDown();
                ActivateAllPuzzleElements();
            }
            else
            {
                FlipButtonUp();
                DeactivateAllPuzzleElements();
            }
        }
    }
}
