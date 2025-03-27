using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStarActionController : MonoBehaviour
{
    /*[SerializeField]*/ StarActions starActions;
    /*[SerializeField]*/ Transform starTransform;
    /*[SerializeField]*/ Transform playerTransform;
    /*[SerializeField]*/ PlayerController playerController;

    [SerializeField] float normalThrowRange = 4;
    [SerializeField] float strongThrowRange = 10;
    [SerializeField] private float starPickupRange = 1.0f;
    [SerializeField] private float recallRange = 4.0f;
    [SerializeField] private float gravityPullRange = 3.0f;
    [SerializeField] private float gravityPullAcceptanceRadius = 0.5f;
    [SerializeField] private float gravityPullSpeed = 5.0f;
    [SerializeField] private float aimSensitivity = 0.5f;

    Vector3 mouseDownPosition;
    Vector3 mouseReleasePosition;

    bool isAiming = false;
    bool strongThrow = false;
    Vector3 throwDirection;
    Vector3 throwTargetDestination;

    void Start()
    {
        GameObject star = GameObject.FindGameObjectWithTag("Star");
        starActions = star.GetComponent<StarActions>();
        starTransform = star.GetComponent<Transform>();

        playerTransform = this.GetComponent<Transform>();
        playerController = this.GetComponent<PlayerController>();
    }

    void Update()
    {
        if (isAiming)
        {
            mouseReleasePosition = Input.mousePosition;
            throwDirection = mouseDownPosition - mouseReleasePosition; // Drag direction
            throwDirection.z = throwDirection.y; // Map vertical screen movement to Z-axis movement
            throwDirection.y = 0; // Keep movement on XZ plane
            
            throwDirection *= aimSensitivity / 100; //controlling the length of the throw was way too sensitive without this

            if(strongThrow && throwDirection.sqrMagnitude > MathF.Pow(strongThrowRange, 2))
            {
                throwDirection = throwDirection.normalized * strongThrowRange;
            }
            else if (!strongThrow && throwDirection.sqrMagnitude > MathF.Pow(normalThrowRange, 2))
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
        //Debug.Log("left mouse down");

        if (starActions.IsOnPlayer)
        {
            isAiming = true;

            mouseDownPosition = Input.mousePosition;
        }
    }
    void OnLeftMouseRelease(InputValue input)
    {
        //Debug.Log("left mouse release");
        isAiming = false;

        if (starActions.IsOnPlayer)
        {
            throwTargetDestination = transform.position + throwDirection;

            starActions.Throw(throwTargetDestination, throwDirection.normalized);
        }
    }
    
    void OnRightMouseDown(InputValue input)
    {
        Debug.Log("right mouse down");

        if (starActions.IsOnPlayer)
        {
            isAiming = true;
            strongThrow = true;

            mouseDownPosition = Input.mousePosition;
        }
    }
    void OnRightMouseRelease(InputValue input)
    {
        Debug.Log("right mouse release");
        isAiming = false;
        strongThrow = false;

        if (starActions.IsOnPlayer)
        {
            throwTargetDestination = transform.position + throwDirection;

            starActions.Throw(throwTargetDestination, throwDirection.normalized);
        }
    }

    void OnGravityPull(InputValue input)
    {
        Debug.Log("gravity pull");

        if (starActions.IsOnPlayer)
        {
            return;
        }

        if (Vector3.Distance(transform.position, starTransform.position) <= gravityPullRange)
        {
            StartCoroutine(GravityPullToDestination(starTransform.position));
        }
    }

    IEnumerator GravityPullToDestination(Vector3 targetDestination)
    {
        Debug.Log("GRAVITY PULLING TO DESTINATION...");

        while (Vector3.Distance(transform.position, targetDestination) > gravityPullAcceptanceRadius)
        {
            Vector3 direction = targetDestination - transform.position;
            direction = direction.normalized;
            
            playerController.InteruptMovement();
            transform.position += direction * gravityPullSpeed * Time.deltaTime;

            yield return null;
        }

        starActions.Recall();
        Debug.Log("GRAVITY PULL DESTINATION REACHED!");
    }
}
