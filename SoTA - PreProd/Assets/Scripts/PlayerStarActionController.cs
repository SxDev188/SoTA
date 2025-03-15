using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStarActionController : MonoBehaviour
{
    [SerializeField] StarActions starActions;
    [SerializeField] Transform starTransform;
    [SerializeField] float normalThrowRange = 4;
    [SerializeField] private float starPickupRange = 1.0f;
    [SerializeField] private float recallRange = 4.0f;
    [SerializeField] private float sensitivity = 0.5f;

    Vector3 mouseDownPosition;
    Vector3 mouseReleasePosition;

    bool isAiming = false;
    Vector3 throwDirection;
    Vector3 throwTargetDestination;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isAiming)
        {
            mouseReleasePosition = Input.mousePosition;
            throwDirection = mouseDownPosition - mouseReleasePosition; // Drag direction
            throwDirection.z = throwDirection.y; // Map vertical screen movement to Z-axis movement
            throwDirection.y = 0; // Keep movement on XZ plane

            throwDirection *= sensitivity / 100; //controlling the length of the throw was way too sensitive without this

            if (throwDirection.sqrMagnitude > MathF.Pow(normalThrowRange, 2))
            {
                throwDirection = throwDirection.normalized * normalThrowRange;
            }
            
            Debug.DrawRay(transform.position, throwDirection, Color.red);
        }
    }

    void OnCarryStarToggle(InputValue input)
    {
        Debug.Log("on carry star toggle");
        if (Vector3.Distance(transform.position, starTransform.position) <= starPickupRange)
        {
            starActions.CarryToggle();
        }
    }
    
    void OnRecallStar(InputValue input)
    {
        Debug.Log("recall");
        if (Vector3.Distance(transform.position, starTransform.position) <= recallRange)
        {
            starActions.Recall();
        }
    }

    void OnLeftMouseDown(InputValue input)
    {
        Debug.Log("left mouse down");

        if (starActions.IsOnPlayer)
        {
            isAiming = true;

            mouseDownPosition = Input.mousePosition;
        }
    }
    void OnLeftMouseRelease(InputValue input)
    {
        Debug.Log("left mouse release");
        isAiming = false;

        if (starActions.IsOnPlayer)
        {
            throwTargetDestination = transform.position + throwDirection;

            starActions.Throw(throwTargetDestination, throwDirection.normalized);
        }

    }
}
