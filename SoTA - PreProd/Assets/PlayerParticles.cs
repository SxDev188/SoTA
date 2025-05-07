using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticles : MonoBehaviour
{
    [SerializeField] PlayerStarActionController playerStarActionController;
    [SerializeField] Transform starTransform;
    [SerializeField] StarActions starActions;
    private bool isBeingGravityPulled;
    private float recallRange;
    private LineRenderer gravityPullLine;
    private EventInstance starShimmerSFX;

    void Start()
    {
        playerStarActionController = FindObjectOfType<PlayerStarActionController>();
        starTransform = GameObject.FindWithTag("Star").transform;
        starActions = GetComponent<StarActions>();

        isBeingGravityPulled = playerStarActionController.IsBeingGravityPulled;
        recallRange = playerStarActionController.RecallRange;

        // Initialize LineRenderer for gravity pull
        gravityPullLine = transform.Find("GravityLine").GetComponent<LineRenderer>();
        gravityPullLine.positionCount = 2; // Two points: player and star
        gravityPullLine.enabled = false; // Start disabled
        gravityPullLine.numCapVertices = 10;
        gravityPullLine.startWidth = 1f;
        gravityPullLine.endWidth = 0.0f; // Adjust the end width as needed
    }

    void Update()
    {
        isBeingGravityPulled = playerStarActionController.IsBeingGravityPulled;

        if (isBeingGravityPulled)
        {
            if (!gravityPullLine.enabled)
            {
                gravityPullLine.enabled = true; // Enable the line renderer
            }

            // Set the positions for the line renderer
            gravityPullLine.SetPosition(0, transform.position); // Start position (Player)
            gravityPullLine.SetPosition(1, starTransform.position); // End position (Star)

            // Adjust width based on distance to star
            float distanceToStar = Vector3.Distance(transform.position, starTransform.position);
            float lineWidth = Mathf.Lerp(0.1f, 0.0f, distanceToStar / 20f); // Change the divisor to control shrinking speed
            gravityPullLine.startWidth = 0.5f; // Start width
            gravityPullLine.endWidth = lineWidth; // End width
        }
        else
        {
            if (gravityPullLine.enabled)
            {
                gravityPullLine.enabled = false; // Disable the line renderer when not being pulled
            }
        }

        // Debug
        Debug.Log("Gravity pull state: " + isBeingGravityPulled);
        Debug.Log("Gravity line found: " + gravityPullLine);
    }
}