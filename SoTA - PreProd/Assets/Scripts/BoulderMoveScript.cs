using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BoulderMoveScript : MonoBehaviour, IInteractable
{
    private float interactionRange = 2f;
    private bool isAttached = false;
    private GameObject player;
    private PlayerController playerController;
    private Vector3 plrHitscan;
    private Rigidbody boulderRigidbody;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();

        boulderRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Level Floor") || collision.gameObject.CompareTag("Abyss") || collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("PressurePlate"))
        {
            return;
        }
        
        if (collision.gameObject.CompareTag("Star") && isAttached && collision.gameObject.GetComponent<StarActions>().IsOnPlayer) //so that carrying the star doesn't block the boulder push
        {
            return;
        }

        Debug.Log("BOULDER IS COLLIDING WITH SOMETHING with tag: " + collision.gameObject.tag);
        Detach();
    }

    private void SnapToFloor()
    {
        if (!isAttached)
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
        Debug.Log("THIS BOULDER IS ATTACHED?: " + isAttached);

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

    private void Attach()
    {
        isAttached = true;
        boulderRigidbody.isKinematic = false;
        this.transform.SetParent(player.transform);
        LockPlayerMovement();
        
        Debug.Log("boulder was attached");
    }
    public void Detach() //Added so when Load can detach the boulder from the player by Linus
    {
        isAttached = false;
        boulderRigidbody.isKinematic = true; //solves jank with boulder pushing away player when walked into
        this.transform.parent = null;
        UnlockPlayerMovement();

        SnapToFloor();

        Debug.Log("boulder was detached");
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
        Debug.Log("Unlocking player movement");
    }
}
