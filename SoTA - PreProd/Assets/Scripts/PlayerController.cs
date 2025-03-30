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
    [SerializeField] float speed = 7.0f;

    [SerializeField] float maxVelocity = 5;
    
    Vector3 MovementInput = Vector3.zero;
    bool isMoving = false;

    [SerializeField] float movementRotationByDegrees = 45;
    Vector3 rotationAxis = Vector3.up;

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
        if (isMoving)
        {
            playerRigidbody.MovePosition(transform.position + transform.forward * MovementInput.magnitude * speed * Time.deltaTime);
        }
    }

    void OnMoveInput(InputValue input)
    {
        isMoving = true;

        Vector2 input2d = input.Get<Vector2>();
        MovementInput = new Vector3(input2d.x, 0, input2d.y);
        MovementInput = RotateVector3(MovementInput, movementRotationByDegrees, rotationAxis);

        LookAtMovementDirection();
    }

    void OnMoveRelease(InputValue input)
    {
        InteruptMovement();
    }
    public void InteruptMovement()
    {
        isMoving = false;
        MovementInput = Vector3.zero;
    }

    void LookAtMovementDirection()
    {
        if (MovementInput != Vector3.zero)
        {
            Vector3 movementDirection = (transform.position + MovementInput) - transform.position;
            Quaternion rotation = Quaternion.LookRotation(movementDirection, rotationAxis);

            transform.rotation = rotation;
        }

        //transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation);         //this line might be needed for controller
    }

    public void LockMovement(Vector3 Axis) //For boulder movement
    {
        MovementInput = Vector3.Scale(MovementInput, Axis);
    }

    public Vector3 RayBoulderInteration(float interactionRange)
    {
        RaycastHit hit;
        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };

        for (int i = 0; i < directions.Length; i++)
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(directions[i]), out hit, interactionRange))
            {
                //Debug.DrawRay(transform.position, transform.TransformDirection(directions[i]) * hit.distance, Color.red);

                if (hit.transform.tag == "Boulder")
                {
                    return directions[i];
                }
            }
        }

        return Vector3.zero;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Spikes"))
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
