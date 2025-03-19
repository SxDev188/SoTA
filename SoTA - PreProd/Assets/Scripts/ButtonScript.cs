using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class ButtonScript : MonoBehaviour, IInteractable
{
    [SerializeField] private List<GameObject> puzzleElements = new List<GameObject>();
    [SerializeField] private bool hasTimer = false;
    [SerializeField] private float totalTimerDuration = 3;

    private bool isActive = false;
    private bool isTimerRunning = false;
    private Transform player;

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Interact()
    {
        if (isActive && isTimerRunning)
        {
            return; //we busy
        }

        if (isActive)
        {
            DeactivateAllPuzzleElements();
            isActive = false;
        }
        else if (!isActive && hasTimer)
        {
            StartTimerForAllPuzzleElements();
        }
        else if (!isActive)
        {
            ActivateAllPuzzleElements();
            isActive = true;
        }
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

    private void ToggleButtonState()
    {
        isActive = !isActive;
    }

    private IEnumerator DeactivateDelayed(IActivatable activatable)
    {
        yield return new WaitForSeconds(totalTimerDuration);
        activatable.Deactivate();
        isActive = false;
        isTimerRunning = false;
    }
}
