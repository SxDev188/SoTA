using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTriggerScript : MonoBehaviour
{
    public Vector2 panDirection;
    public List<Vector2> requiredEntryDirections;

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

    private bool IsMovingMostlyInRequiredDirection(Vector2 moveDirection)
    {
        foreach (var requiredDirection in requiredEntryDirections)
        {
            float dotProduct = Vector2.Dot(moveDirection.normalized, requiredDirection.normalized);
            if (dotProduct > 0.7f)  
            {
                return true; 
            }
        }
        return false; 
    }
}