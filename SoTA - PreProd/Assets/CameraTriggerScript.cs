using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTriggerScript : MonoBehaviour
{
    public Vector2 panDirection;
    public Vector2 requiredEntryDirection;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null && IsMovingMostlyInRequiredDirection(player.GetLastMoveDirection()))
            {
                //Debug.Log("Trigger Activated!");
                CameraPanScript.Instance.PanCamera(panDirection);
            }
        }
    }

    private bool IsMovingMostlyInRequiredDirection(Vector2 moveDirection) //Because we have diagonal movement
    {
        float dotProduct = Vector2.Dot(moveDirection.normalized, requiredEntryDirection.normalized);
        return dotProduct > 0.7f; 
    }
}
