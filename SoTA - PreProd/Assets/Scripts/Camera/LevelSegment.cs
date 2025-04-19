using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSegment : MonoBehaviour
{
    [SerializeField] Vector2 segmentPosition;
    PlayerSegment playerSegment;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerSegment = other.GetComponent<PlayerSegment>();
            playerSegment.AddSegment(this);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerSegment = other.GetComponent<PlayerSegment>();
            playerSegment.RemoveSegment(this);
        }
    }
}
