using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class ButtonScript : MonoBehaviour, IInteractable
{
    [SerializeField] private List<GameObject> puzzleElements = new List<GameObject>();

    private bool isActive = false;
    private Transform player;

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Interact()
    {
        foreach (GameObject puzzleElement in puzzleElements)
        {
            IActivatable activatable = puzzleElement.GetComponent<IActivatable>();
            if (activatable != null)
            {
                if (isActive)
                {
                    activatable.Deactivate();
                }
                else
                {
                    activatable.Activate();
                }
            }
        }

        isActive = !isActive;
    }
}
