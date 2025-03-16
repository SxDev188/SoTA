using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterController characterController;
    [SerializeField] float speed = 50.0f;
    [SerializeField] public int health = 10;
    Vector3 MovementInput = Vector3.zero;
    bool isMoving = false;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (isMoving) {
            characterController.SimpleMove(MovementInput * speed * Time.deltaTime);
        }
    }

    void OnMoveInput(InputValue input)
    {
        isMoving = true;
        Vector2 input2d = input.Get<Vector2>();
        MovementInput = new Vector3(input2d.x, MovementInput.y, input2d.y);
    }

    void OnMoveRelease(InputValue input)
    {
        isMoving = false;
        MovementInput = Vector3.zero;
    }

    public void InteruptMovement()
    {
        //this method is needed to interupt SimpleMove since it made it
        //impossible to manipulate the players transform directly for the gravity pull
        isMoving = false;
    }
}
