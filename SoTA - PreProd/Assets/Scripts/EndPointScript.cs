using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EndPointScript : MonoBehaviour, IInteractable
{
    // Start is called before the first frame update
    void Start()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Interact();
        }
    }
    public void Interact()
    {
        Application.Quit();
        Debug.Log("Game Ended");
    }
}
