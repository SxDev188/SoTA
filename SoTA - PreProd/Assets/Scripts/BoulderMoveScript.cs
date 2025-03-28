using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public class BoulderMoveScript : MonoBehaviour, IInteractable
{
    private float interactionRange = 2f;
    private bool isMoving = false;
    private GameObject player;
    private PlayerController playerController;
    private Vector3 plrHitscan;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    private void Update()
    {
        plrHitscan = playerController.RayBoulderInteraction(interactionRange);

        if (isMoving)
        {
            this.transform.SetParent(player.transform);

            if(plrHitscan == Vector3.forward || plrHitscan == Vector3.back)
            {
                playerController.LockMovement(Vector3.forward);
            }
            if(plrHitscan == Vector3.right || plrHitscan == Vector3.left)
            {
                playerController.LockMovement(Vector3.right);
            }
        }
        else
        {
            if (playerController.GetIsMovementLocked()) //this block was added because locking movement did not work as usual after the player movement was updated
            {
                playerController.UnlockMovement();
                Debug.Log("Unlocking player movement");
            }

            SnapToFloor();
            this.transform.parent = null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag != "Level Floor" || collision.gameObject.tag != "Player" || collision.gameObject.tag != "PressurePlate")
        {
            isMoving = false;
        }
    }

    private void SnapToFloor()
    {
        if (!isMoving)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1f))
            {
                Vector3 targetPosition = new Vector3(hit.transform.position.x, transform.position.y, hit.transform.position.z);
                transform.position = targetPosition;
            }
        }
    }

    private bool PlayerIsClose()
    {
        return Vector3.Distance(transform.position, player.transform.position) <= interactionRange;
    }

    public void Interact()
    {
        if (PlayerIsClose() && plrHitscan != Vector3.zero)
        {
            isMoving = !isMoving;
        }
    }
}
