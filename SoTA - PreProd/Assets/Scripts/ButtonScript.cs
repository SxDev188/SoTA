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

    public bool IsActive { get { return isPushed; } }

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Interact()
    {
        if (isPushed && isTimerRunning)
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ButtonPushedFailSound, this.transform.position);
            return; //we busy
        }

        if (!isPushed && !hasTimer)
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ButtonPushedDownSound, this.transform.position);
            ActivateAllPuzzleElements();
            isPushed = true;
        }
        else if (!isPushed && hasTimer)
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ButtonPushedDownSound, this.transform.position);
            StartTimerForAllPuzzleElements();
            isPushed = true;
        }
        else if (isPushed && !isTimerRunning)
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ButtonPushedUpSound, this.transform.position);
            DeactivateAllPuzzleElements();
            isPushed = false;
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

    private IEnumerator DeactivateDelayed(IActivatable activatable)
    {
        yield return new WaitForSeconds(totalTimerDuration);
        activatable.Deactivate();
        isPushed = false;
        isTimerRunning = false;
    }
    private void ToggleButtonState()
    {
        isPushed = !isPushed;
    }
}
