using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterController characterController;
    [SerializeField] float speed = 50.0f;
    Vector3 MovementInput = Vector3.zero;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        characterController.SimpleMove(MovementInput * speed * Time.deltaTime);
    }

    void OnMoveInput(InputValue input)
    {
        Vector2 input2d = input.Get<Vector2>();
        MovementInput = new Vector3(input2d.x, MovementInput.y, input2d.y);
    }

    void OnMoveRelease(InputValue input)
    {
        MovementInput = Vector3.zero;
    }

    


}
