using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class Linus_PlayerStarActionController : MonoBehaviour
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
    [SerializeField] private bool recallAllowed = false;
    [SerializeField] private bool gravityPullAllowed = false;
    [SerializeField] private bool strongThrowAllowed = false;
    float healthChangeTimer = 0.0f;

    [SerializeField] float aimRotationByDegrees = 45;
    Vector3 rotationAxis = Vector3.up;

    Vector3 mouseDownPosition;
    Vector3 mouseReleasePosition;

    bool isAiming = false;
    bool strongThrow = false;
    Vector3 throwDirection;
    Vector3 throwTargetDestination;

    private LineRenderer lineRenderer;

    Vector3 aimInput;
    void Start()
    {
        GameObject star = GameObject.FindGameObjectWithTag("Star");
        starActions = star.GetComponent<StarActions>();
        starTransform = star.GetComponent<Transform>();

        playerTransform = this.GetComponent<Transform>();
        playerController = this.GetComponent<PlayerController>();

        InitializeLineRenderer();
    }

    void Update()
    {
        if (isAiming)
        {
            //mouseReleasePosition = Input.mousePosition;
            //throwDirection = mouseDownPosition - mouseReleasePosition; // Drag direction
            //throwDirection.z = throwDirection.y; // Map vertical screen movement to Z-axis movement
            //throwDirection.y = 0; // Keep movement on XZ plane

            throwDirection = aimInput;
            //throwDirection *= aimSensitivity / 100; //controlling the length of the throw was way too sensitive without this

            

            if (strongThrow && throwDirection.sqrMagnitude > MathF.Pow(strongThrowRange, 2))
            {
                throwDirection = throwDirection.normalized * strongThrowRange;
            }
            else if (!strongThrow && throwDirection.sqrMagnitude > MathF.Pow(normalThrowRange, 2))
            {
                throwDirection = throwDirection.normalized * normalThrowRange;
            }

            throwDirection = HelperScript.RotateVector3(throwDirection, aimRotationByDegrees, rotationAxis);

            DrawAimLine();
            //Debug.DrawRay(transform.position, throwDirection, Color.red);
        }
        else
        {
            HideAimLine();
        }

        healthChangeTimer += Time.deltaTime;
        ManagePlayerHealth();
    }

    void DrawAimLine()
    {
        if (!lineRenderer.enabled)
        {
            lineRenderer.enabled = true;
        }

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position + throwDirection);
    }

    void HideAimLine()
    {
        if (lineRenderer.enabled)
        {
            lineRenderer.enabled = false;
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
        if (!recallAllowed)
        {
            return;
        }

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

        if (starActions.IsOnPlayer && isAiming)
        {
            isAiming = false;

            throwTargetDestination = transform.position + throwDirection;


            starActions.Throw(throwTargetDestination, throwDirection.normalized);

            //this was just for debug
            //Vector3 testingTargetPosition = new Vector3(5, 7, 5);
            //starActions.Throw(testingTargetPosition, testingTargetPosition.normalized);
        }
    }
    void OnRightMouseDown(InputValue input)
    {
        if (!strongThrowAllowed)
        {
            return;
        }

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
        if (!strongThrowAllowed)
        {
            return;
        }

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
        if (!gravityPullAllowed)
        {
            return;
        }

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
    private void InitializeLineRenderer()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.startWidth = 0.03f;
        lineRenderer.endWidth = 0.03f;
        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }
    
    void OnAimInput(InputValue input)
    {
        isAiming = true;

        Vector2 input2d = input.Get<Vector2>();
        aimInput = new Vector3(input2d.x, 0, input2d.y) * normalThrowRange;

    }
    void OnAimRelease(InputValue input)
    {
        ThrowStar();
    }

    void ThrowStar()
    {
        if (starActions.IsOnPlayer && isAiming)
        {
            isAiming = false;

            throwTargetDestination = transform.position + throwDirection;


            starActions.Throw(throwTargetDestination, throwDirection.normalized);

            //this was just for debug
            //Vector3 testingTargetPosition = new Vector3(5, 7, 5);
            //starActions.Throw(testingTargetPosition, testingTargetPosition.normalized);
        }
    }
    void ManagePlayerHealth()
    {
        float changeHealthAtTime = 1.0f;

        if (playerController.currentHealth > 0 && starActions.IsOnPlayer == false && healthChangeTimer >= changeHealthAtTime)
        {
            playerController.currentHealth--;
            healthChangeTimer = 0.0f;
            Debug.Log("health managed, health at " + playerController.currentHealth);
        }
        if (playerController.currentHealth < playerController.maxHealth && starActions.IsOnPlayer && healthChangeTimer >= changeHealthAtTime)
        {
            playerController.currentHealth++;
            healthChangeTimer = 0.0f;
            Debug.Log("health managed, health at " + playerController.currentHealth);
        }
        if (healthChangeTimer >= changeHealthAtTime)
        {
            healthChangeTimer = 0.0f;
        }

    }
}
