using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PressurePlateScript : MonoBehaviour
{
    [SerializeField] private List<GameObject> puzzleElements = new List<GameObject>();

    private List<GameObject> objectsOnPlate = new List<GameObject>();
    private bool isPushedDown = false;

    private EventInstance pressurePlateSFX;


    private void Start()
    {
        pressurePlateSFX = AudioManager.Instance.CreateInstance(FMODEvents.Instance.ButtonSFX);

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Star") || other.CompareTag("Boulder"))
        {
            objectsOnPlate.Add(other.gameObject);
        }

        if (objectsOnPlate.Count > 0)
        {
            if (!isPushedDown)
            {
                isPushedDown = true;
                pressurePlateSFX.setParameterByNameWithLabel("ButtonPushState", "PushDown");
                pressurePlateSFX.start();

                Interact();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Star") || other.CompareTag("Boulder"))
        {
            objectsOnPlate.Remove(other.gameObject);
        }

        if (objectsOnPlate.Count <= 0)
        {
            isPushedDown = false;
            pressurePlateSFX.setParameterByNameWithLabel("ButtonPushState", "PushUp");
            pressurePlateSFX.start();


            Interact();
        }
    }

    public void Interact()
    {
        foreach (GameObject puzzleElement in puzzleElements)
        {
            IActivatable activatable = puzzleElement.GetComponent<IActivatable>();
            if (activatable != null)
            {
                if (isPushedDown)
                {
                    activatable.Activate();
                }
                else
                {
                    activatable.Deactivate();
                }
            }
        }
    }
}
