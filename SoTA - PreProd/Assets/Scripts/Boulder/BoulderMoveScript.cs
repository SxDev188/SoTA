using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BoulderMoveScript : MonoBehaviour, IInteractable
{
    private static BoulderMoveScript currentlyActiveBoulder = null; //used to fix bug where player could attach to two boulders at once

    private float interactionRange = 2f;
    private bool isAttached = false;
    private GameObject player;
    private PlayerController playerController;
    private Vector3 plrHitscan;
    private Rigidbody boulderRigidbody;

    Vector3 offsetToPlayer;

    private BoulderStarPushScript boulderStarPushScript;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();

        boulderRigidbody = GetComponent<Rigidbody>();
        boulderStarPushScript = GetComponent<BoulderStarPushScript>();
    }

    private void Update()
    {
        if (isAttached && !boulderStarPushScript.IsBeingPushed)
        {
            if (playerController.GetBoulderPushDirection() != Vector3.zero)
            {
                boulderStarPushScript.PlayerPushInDirection(playerController.GetBoulderPushDirection());
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            SnapToFloor();
        }

        if (isAttached)
        {
            //player adheres to the boulders position, respecting the offset that was there when they attached
            player.transform.position = transform.position - offsetToPlayer; 
        }

        ////old boulder movement code
        //if (isAttached)
        //{
        //    transform.position = player.transform.position + offsetToPlayer; //boulder adheres to players position
        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!isAttached)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Level Floor") || collision.gameObject.CompareTag("Abyss") || collision.gameObject.CompareTag("Player"))
        {
            return;
        }
        
        if (collision.gameObject.CompareTag("AntiStarZone") || collision.gameObject.CompareTag("PressurePlate"))
        {
            return;
        }

        if (collision.gameObject.CompareTag("Star") && collision.gameObject.GetComponent<StarActions>().IsOnPlayer) //so that carrying the star doesn't block the boulder push
        {
            return;
        }

        //Debug.Log("BOULDER IS COLLIDING WITH SOMETHING with tag: " + collision.gameObject.tag);
        Detach();
    }

    public void SnapToFloor()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1f))
        {
            Vector3 targetPosition = new Vector3(hit.transform.position.x, transform.position.y, hit.transform.position.z);
            transform.position = targetPosition;
        }
    }

    public void Interact()
    {
        if(currentlyActiveBoulder != null && currentlyActiveBoulder != this) //fixes bug where player could sometimes attach to two boulders at once
        {
            currentlyActiveBoulder.Detach();
            return;
        }

        if (isAttached)
        {
            Detach();
            return;
        }

        if (!PlayerIsClose())
        {
            return;
        }

        //ask player if they can find this boulder and if so, which direction the player is comming from
        plrHitscan = playerController.RayBoulderInteraction(interactionRange, this.gameObject);

        if (plrHitscan == Vector3.zero)
        {
            return;
        }

        if (!isAttached)
        {
            Attach();
        }
    }

    private bool PlayerIsClose()
    {
        return Vector3.Distance(transform.position, player.transform.position) <= interactionRange;
    }

    private void Attach()
    {
        isAttached = true;
        boulderRigidbody.isKinematic = false;
        LockPlayerMovement();

        offsetToPlayer = transform.position - player.transform.position;

        currentlyActiveBoulder = this;

        //Debug.Log("boulder was attached");

        playerController.AttachToBoulder();
    }
    public void Detach() //Added so when Load can detach the boulder from the player by Linus
    {
        isAttached = false;
        boulderRigidbody.isKinematic = true; //solves jank with boulder pushing away player when walked into
        UnlockPlayerMovement();

        SnapToFloor();

        currentlyActiveBoulder = null;

        //Debug.Log("boulder was detached");

        playerController.DetachFromBoulder();

    }

    private void LockPlayerMovement()
    {
        if (plrHitscan == Vector3.forward || plrHitscan == Vector3.back)
        {
            playerController.LockMovement(Vector3.forward);
        }
        if (plrHitscan == Vector3.right || plrHitscan == Vector3.left)
        {
            playerController.LockMovement(Vector3.right);
        }
    }
    
    private void UnlockPlayerMovement()
    {
        playerController.UnlockMovement();
    }
}
