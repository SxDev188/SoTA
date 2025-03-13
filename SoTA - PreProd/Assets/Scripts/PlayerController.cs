using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    CharacterController characterController;

    [SerializeField] StarActions starActions;
    [SerializeField] float normalThrowRange = 4;

    [SerializeField]
    float speed = 50.0f;

    bool isAiming = false;
    Vector3 throwDirection;
    Vector3 throwTargetDestination;


    Vector3 MovementInput = Vector3.zero;

    Vector3 mouseDownPosition;
    Vector3 mouseReleasePosition;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        characterController.SimpleMove(MovementInput * speed * Time.deltaTime);

        if (isAiming)
        {
            mouseReleasePosition = Input.mousePosition;
            throwDirection = mouseDownPosition - mouseReleasePosition; // Drag direction
            throwDirection.z = throwDirection.y; // Map vertical screen movement to Z-axis movement
            throwDirection.y = 0; // Keep movement on XZ plane

            throwTargetDestination = throwDirection * normalThrowRange;
            Debug.DrawRay(transform.position, throwTargetDestination / 100, Color.red);
        }
    }

    void OnMoveInput(InputValue input)
    {
        Vector2 input2d = input.Get<Vector2>();
        //Debug.Log(input.ToString() + ", " + input2d.ToString());

        MovementInput = new Vector3(input2d.x, MovementInput.y, input2d.y);
    }

    void OnMoveRelease(InputValue input)
    {
        MovementInput = Vector3.zero;
        Debug.Log("movement release");

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
            mouseReleasePosition = Input.mousePosition;

            starActions.Throw(throwTargetDestination);
        }

    }


}
