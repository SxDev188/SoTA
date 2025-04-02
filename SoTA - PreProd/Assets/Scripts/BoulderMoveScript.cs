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
        Debug.Log("PRE UPDATE: " + plrHitscan);

        plrHitscan = playerController.RayBoulderInteraction(interactionRange);

        Debug.Log("POST UPDATE: " + plrHitscan);


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
            //this block was added because locking movement did not work as usual after the player movement was updated
            if (playerController.GetIsMovementLocked() && transform.parent == null) //null check so it doesn't run every frame
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
        //Debug.Log("Distance:");

        //Debug.Log(Vector3.Distance(transform.position, player.transform.position));

        return Vector3.Distance(transform.position, player.transform.position) <= interactionRange;
    }

    public void Interact()
    {
        Debug.Log("boulder was interacted with");
        Debug.Log(plrHitscan);

        if (PlayerIsClose() && plrHitscan != Vector3.zero)
        {

            isMoving = !isMoving;
        }
    }

    //Added so when Load can detach the boulder from the player by Linus
    public void Detach()
    {
        isMoving = false;
    }
}
