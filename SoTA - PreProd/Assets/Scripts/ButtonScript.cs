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

        foreach (GameObject puzzleElement in puzzleElements)
        {
            IActivatable activatable = puzzleElement.GetComponent<IActivatable>();
            if (activatable != null)
            {
                if (isActive)
                {
                    if (!isTimerRunning)
                    {
                        activatable.Deactivate();
                    }
                }
                else
                {
                    if (hasTimer)
                    {
                        StartCoroutine(DeactivateDelayed(activatable));
                        isTimerRunning = true;
                    }
                    activatable.Activate();
                }
            }
        }
        ToggleButtonState();
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
