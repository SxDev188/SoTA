using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour, IInteractable
{
    [SerializeField] private List<GameObject> puzzleElements = new List<GameObject>();
    [SerializeField] private bool hasTimer = false;
    [SerializeField] private float totalTimerDuration = 3;

    private Transform button;
    private bool isPushed = false;
    private bool isTimerRunning = false;

    private EventInstance buttonSFX;
    private EventInstance timerTickingSFX;

    private ParticleSystem buttonParticles;

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
        buttonParticles = GetComponentInChildren<ParticleSystem>();
        FindParticleColor();

        if (button == null)
        {
            Debug.LogError("Button_connection child not found! Check the hierarchy.");
        }

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
    }
    private void ToggleButtonState()
    {
        isPushed = !isPushed;
    }

    private void FlipButtonDown()
    {
        button.localRotation = Quaternion.Euler(180, 0, 0);
        StartCoroutine(PlayAndStopBurst());
    }

    private void FlipButtonUp()
    {
        button.localRotation = Quaternion.Euler(0, 0, 0);
        StartCoroutine(PlayAndStopBurst());
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
                StopAllCoroutines();
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

    IEnumerator PlayAndStopBurst()
    {
        buttonParticles.Play();
        yield return new WaitForSeconds(0.1f); // wait for particles to spawn
        buttonParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private void FindParticleColor()
    {
        Transform buttonConnection = transform.Find("Button/Button_connection");
        Transform buttonParticlesTransform = transform.Find("ButtonParticles");

        if (buttonConnection != null && buttonParticlesTransform != null)
        {
            MeshRenderer sourceRenderer = buttonConnection.GetComponent<MeshRenderer>();
            ParticleSystemRenderer psRenderer = buttonParticlesTransform.GetComponent<ParticleSystemRenderer>();

            if (sourceRenderer != null && psRenderer != null)
            {
                psRenderer.material = sourceRenderer.sharedMaterial;
            }
            else
            {
                Debug.LogWarning("MeshRenderer or ParticleSystemRenderer not found.");
            }
        }
        else
        {
            Debug.LogError("Button_connection or ButtonParticles not found in the hierarchy.");
        }
    }
}
