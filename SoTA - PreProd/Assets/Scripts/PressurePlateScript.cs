using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateScript : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject objectToMoveDown;
    [SerializeField] private GameObject objectToMoveUp;
    [SerializeField] private GameObject objectToMoveLeft;
    [SerializeField] private GameObject objectToMoveRight;
    [SerializeField] private float moveDistance = 1f;
    [SerializeField] private float moveSpeed = 2f;

    private Vector3 initialPositionDown, targetPositionDown;
    private Vector3 initialPositionUp, targetPositionUp;
    private Vector3 initialPositionLeft, targetPositionLeft;
    private Vector3 initialPositionRight, targetPositionRight;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetObjectPositions();
            Interact();
        }
    }

    private void SetObjectPositions()
    {
        if (objectToMoveDown != null)
        {
            initialPositionDown = objectToMoveDown.transform.position;
            targetPositionDown = initialPositionDown + Vector3.down * moveDistance;
        }

        if (objectToMoveUp != null)
        {
            initialPositionUp = objectToMoveUp.transform.position;
            targetPositionUp = initialPositionUp + Vector3.up * moveDistance;
        }

        if (objectToMoveLeft != null)
        {
            initialPositionLeft = objectToMoveLeft.transform.position;
            targetPositionLeft = initialPositionLeft + Vector3.left * moveDistance;
        }

        if (objectToMoveRight != null)
        {
            initialPositionRight = objectToMoveRight.transform.position;
            targetPositionRight = initialPositionRight + Vector3.right * moveDistance;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InteractReset();
        }
    }

    public void Interact()
    {
            if (objectToMoveDown != null) StartCoroutine(MoveObject(objectToMoveDown, targetPositionDown));
            if (objectToMoveUp != null) StartCoroutine(MoveObject(objectToMoveUp, targetPositionUp));
            if (objectToMoveLeft != null) StartCoroutine(MoveObject(objectToMoveLeft, targetPositionLeft));
            if (objectToMoveRight != null) StartCoroutine(MoveObject(objectToMoveRight, targetPositionRight));
    }

    public void InteractReset()
    {
        if (objectToMoveDown != null) StartCoroutine(MoveObject(objectToMoveDown, initialPositionDown));
        if (objectToMoveUp != null) StartCoroutine(MoveObject(objectToMoveUp, initialPositionUp));
        if (objectToMoveLeft != null) StartCoroutine(MoveObject(objectToMoveLeft, initialPositionLeft));
        if (objectToMoveRight != null) StartCoroutine(MoveObject(objectToMoveRight, initialPositionRight));
    }

    IEnumerator MoveObject(GameObject obj, Vector3 targetPosition)
    {
        while (Vector3.Distance(obj.transform.position, targetPosition) > 0.01f)
        {
            obj.transform.position = Vector3.MoveTowards(obj.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        obj.transform.position = targetPosition; // Ensure exact final position
    }
}
