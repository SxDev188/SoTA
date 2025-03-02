using System.Collections;
using UnityEngine;

public class ButtonScript : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject objectToMoveDown;
    [SerializeField] private GameObject objectToMoveUp;
    [SerializeField] private GameObject objectToMoveLeft;
    [SerializeField] private GameObject objectToMoveRight;
    [SerializeField] private float moveDistance = 1f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float interactionRange = 1f;

    private Vector3 initialPositionDown, targetPositionDown;
    private Vector3 initialPositionUp, targetPositionUp;
    private Vector3 initialPositionLeft, targetPositionLeft;
    private Vector3 initialPositionRight, targetPositionRight;

    private bool hasMoved = false;
    private Transform player;

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        SetObjectPositions();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && PlayerIsClose())
        {
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

    public void Interact()
    {
        if (!hasMoved)
        {
            if (objectToMoveDown != null) StartCoroutine(MoveObject(objectToMoveDown, targetPositionDown));
            if (objectToMoveUp != null) StartCoroutine(MoveObject(objectToMoveUp, targetPositionUp));
            if (objectToMoveLeft != null) StartCoroutine(MoveObject(objectToMoveLeft, targetPositionLeft));
            if (objectToMoveRight != null) StartCoroutine(MoveObject(objectToMoveRight, targetPositionRight));
        }
        else
        {
            if (objectToMoveDown != null) StartCoroutine(MoveObject(objectToMoveDown, initialPositionDown));
            if (objectToMoveUp != null) StartCoroutine(MoveObject(objectToMoveUp, initialPositionUp));
            if (objectToMoveLeft != null) StartCoroutine(MoveObject(objectToMoveLeft, initialPositionLeft));
            if (objectToMoveRight != null) StartCoroutine(MoveObject(objectToMoveRight, initialPositionRight));
        }

        hasMoved = !hasMoved;
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

    private bool PlayerIsClose()
    {
        return Vector3.Distance(transform.position, player.position) <= interactionRange;
    }
}
