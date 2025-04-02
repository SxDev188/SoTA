using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using FMOD.Studio;
using Unity.VisualScripting;
using UnityEngine.Animations;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public int maxHealth = 10;
    public int currentHealth;

    Rigidbody playerRigidbody;
    [SerializeField] float moveSpeed = 7.0f;
    [SerializeField] float boulderPushSpeed = 3.0f;

    //[SerializeField] float maxVelocity = 5;
    
    Vector3 movementInput = Vector3.zero;
    bool isMoving = false;
    bool isMovementLocked = false;
    Vector3 movementLockAxis;

    [SerializeField] float movementRotationByDegrees = 45;
    Vector3 rotationAxis = Vector3.up;
    Vector3 movementDirection;

    private EventInstance playerSlither; //Audio

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        playerSlither = AudioManager.Instance.CreateInstance(FMODEvents.Instance.SlitherSound);
        currentHealth = maxHealth;
    }

    void Update()
    {
        UpdateSound();
    }

    private void FixedUpdate()
    {
        if (isMovementLocked && movementLockAxis != Vector3.zero)
        {
            movementInput = Vector3.Scale(movementInput, movementLockAxis);
        }

        if (isMovementLocked && isMoving) //aka is pushing/pulling boulder
        {
            //this movement does not depend on where player is facing, only movementInput
            playerRigidbody.MovePosition(transform.position + movementInput * boulderPushSpeed * Time.deltaTime);
        }
        else if (isMoving)
        {
            //this ONLY MOVES FORWARD, direction is determined by where character is looking
            playerRigidbody.MovePosition(transform.position + transform.forward * movementInput.magnitude * moveSpeed * Time.deltaTime);
        }
    }

    void OnMoveInput(InputValue input)
    {
        isMoving = true;

        Vector2 input2d = input.Get<Vector2>();
        movementInput = new Vector3(input2d.x, 0, input2d.y);

        if (!isMovementLocked)
        {
            movementInput = RotateVector3(movementInput, movementRotationByDegrees, rotationAxis);
            LookAtMovementDirection();
        }
    }

    void OnMoveRelease(InputValue input)
    {
        InteruptMovement();
    }

    public void InteruptMovement()
    {
        isMoving = false;
        movementInput = Vector3.zero;
    }

    void LookAtMovementDirection()
    {
        if (movementInput != Vector3.zero)
        {
            movementDirection = (transform.position + movementInput) - transform.position;
            Quaternion rotation = Quaternion.LookRotation(movementDirection, rotationAxis);

            transform.rotation = rotation;
        }

        //transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation);         //this line might be needed for controller
    }

    public void LockMovement(Vector3 axis) //For boulder movement
    {
        isMovementLocked = true;
        movementLockAxis = axis;

        //movementInput = Vector3.Scale(movementInput, axis);
    }

    public void UnlockMovement()
    {
        isMovementLocked = false;
        movementLockAxis = Vector3.zero;
        InteruptMovement();
    }

    public bool GetIsMovementLocked()
    {
        return isMovementLocked;
    }

    public Vector3 RayBoulderInteraction(float interactionRange, GameObject interactedBoulder)
    {
        RaycastHit hit;
        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };

        for (int i = 0; i < directions.Length; i++)
        {
            //here I removed the transform.TransformDirection() so that the raycast ignores the player characters orientation/rotation - goobie
            //if (Physics.Raycast(transform.position, transform.TransformDirection(directions[i]), out hit, interactionRange))
            //{
            //    Debug.DrawRay(transform.position, transform.TransformDirection(directions[i]) * hit.distance, Color.red);

            //    if (hit.transform.tag == "Boulder")
            //    {
            //        return directions[i];
            //    }
            //}

            //if (Physics.Raycast(transform.position, directions[i], out hit, interactionRange))
            //{
            //    Debug.DrawRay(transform.position, directions[i] * hit.distance, Color.red);

            //    if (hit.transform.tag == "Boulder")
            //    {
            //        return directions[i];
            //    }
            //}

            if (Physics.Raycast(transform.position, directions[i], out hit, interactionRange))
            {
                Debug.DrawRay(transform.position, directions[i] * hit.distance, Color.red);

                if (hit.transform.gameObject == interactedBoulder && hit.transform.tag == "Boulder")
                {
                    return directions[i];
                }
            }
        }

        //means the boulder will not be attached to the player
        return Vector3.zero;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Spikes") || other.CompareTag("Abyss"))
        {
            currentHealth = 0;
        }
    }

    private void UpdateSound()
    {
        if (isMoving)
        {
            PLAYBACK_STATE playbackState;
            playerSlither.getPlaybackState(out playbackState);

            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                playerSlither.start();
            }
        }
    }

    private Vector3 RotateVector3(Vector3 vectorToRotate, float degrees, Vector3 rotationAxis)
    {
        Vector3 rotatedVector = Quaternion.AngleAxis(degrees, rotationAxis) * vectorToRotate;
        return rotatedVector;
    }

    //void TruncateVelocity()
    //{
    //    //does not seem to be working properly
    //    if (playerRigidbody.velocity.magnitude > maxVelocity)
    //    {
    //        playerRigidbody.velocity = playerRigidbody.velocity.normalized * maxVelocity;
    //    }
    //}
}
