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
    Rigidbody playerRigidbody;
    [SerializeField] float speed = 50.0f;
    [SerializeField] public int health = 10;
    Vector3 MovementInput = Vector3.zero;
    bool isMoving = false;

    [SerializeField] float maxVelocity = 5;

    private EventInstance playerSlither; //Audio

    [SerializeField] float movementRotationByDegrees = 45;
    Vector3 rotationAxis = Vector3.up;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        playerSlither = AudioManager.Instance.CreateInstance(FMODEvents.Instance.SlitherSound);
    }

    void Update()
    {
        //TruncateVelocity();

        UpdateSound();
    }

    private void FixedUpdate()
    {
        playerRigidbody.velocity = MovementInput * speed * Time.deltaTime;
    }

    void TruncateVelocity()
    {
        //does not seem to be working properly
        if (playerRigidbody.velocity.magnitude > maxVelocity)
        {
            playerRigidbody.velocity = playerRigidbody.velocity.normalized * maxVelocity;
        }
    }

    void OnMoveInput(InputValue input)
    {
        isMoving = true;
        Vector2 input2d = input.Get<Vector2>();
        MovementInput = new Vector3(input2d.x, MovementInput.y, input2d.y);

        MovementInput = RotateVector3(MovementInput, movementRotationByDegrees, rotationAxis);
    }

    void OnMoveRelease(InputValue input)
    {
        isMoving = false;
        MovementInput = Vector3.zero;
    }

    public void LockMovement(Vector3 Axis)
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
            health = 0;
        }
    }

    public void InteruptMovement()
    {
        //this method is needed to interupt SimpleMove since it made it
        //impossible to manipulate the players transform directly for the gravity pull
        isMoving = false;
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
}
