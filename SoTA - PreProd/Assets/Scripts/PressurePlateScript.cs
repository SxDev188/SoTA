using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PressurePlateScript : MonoBehaviour, IInteractable
{
    [SerializeField] private List<GameObject> puzzleElements = new List<GameObject>();

    private List<GameObject> objectsOnPlate = new List<GameObject>();
    private bool isPushedDown = false;

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
