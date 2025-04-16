using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderPushController : MonoBehaviour
{
    //this script contains things that are common between star push and player push

    [SerializeField] bool debugMode = false; //shows the raycast checking for valid push position
    [SerializeField] Vector3 raycastOffset = new Vector3(0, -0.4f, 0); //offsets the raycast checking for valid push position, without this it misses the spikes capsule collider as it is too low to the ground

    [SerializeField] float distanceToCheckForGroundBelowBoulder = 1f;
    [SerializeField] float pushDestinationAcceptanceRadius = 0.01f;

    public float PushDestinationAcceptanceRadius { get { return pushDestinationAcceptanceRadius; } }

    BoulderStarPushScript boulderStarPushScript;
    BoulderPlayerPushScript boulderPlayerPushScript;
    BoulderController boulderController;

    private void Start()
    {
        boulderStarPushScript = GetComponent<BoulderStarPushScript>();
        boulderPlayerPushScript = GetComponent<BoulderPlayerPushScript>();
        boulderController = GetComponent<BoulderController>();
    }

    public bool IsBeingPushed
    {
        get
        {
            if(boulderStarPushScript == null || boulderPlayerPushScript == null)
                return false;
            else if (boulderStarPushScript.IsBeingStarPushed || boulderPlayerPushScript.IsBeingPlayerPushed)
                return true;
            else
                return false;
        }
    }

    public bool CheckForValidPushDestination(Vector3 direction, float distance)
    {
        RaycastHit hit;

        if (debugMode)
        {
            Debug.DrawRay(transform.position + raycastOffset, direction, Color.red, 1.0f);
        }

        if (!Physics.Raycast(transform.position + direction * distance, Vector3.down, out hit, distanceToCheckForGroundBelowBoulder))
        {
            //checks if there is ground below the target destination to stop boulder from being star pushed into the abyss
            return false;
        }

        if (Physics.Raycast(transform.position + raycastOffset, direction, out hit, distance)) //to stop boulder from being star pushed into another object
        {
            //this is way too long... should be fixed after VS1
            if (!hit.collider.gameObject.CompareTag("Abyss") && !hit.collider.gameObject.CompareTag("Level Floor") && !hit.collider.gameObject.CompareTag("BoulderSide") && !hit.collider.gameObject.CompareTag("PressurePlate") && !hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.CompareTag("AntiStarZone") && !hit.collider.gameObject.CompareTag("CameraPan"))
            {
                //add tags here that you want boulder to ignore, but remember to also add them in the OnCollisionEnter check

                Debug.Log("RAYCAST HIT SOMETHING WITH TAG: " + hit.collider.gameObject.tag);

                return false;
            }
        }

        return true;
    }

    void StopBoulderPush()
    {
        if (boulderStarPushScript.IsBeingStarPushed)
        {
            boulderStarPushScript.StopStarPush();
        }

        if (boulderPlayerPushScript.IsBeingPlayerPushed)
        {
            boulderPlayerPushScript.StopPlayerPush();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Abyss")
        {
            return;
        }

        if (collision.gameObject.tag == "Level Floor")
        {
            return;
        }

        if (collision.gameObject.CompareTag("AntiStarZone"))
        {
            return;
        }

        if (boulderStarPushScript.IsBeingStarPushed)
        {
            //here you can add checks specific to star push 

            if (collision.gameObject.tag == "Star")
            {
                return;
            }
        }

        if (boulderPlayerPushScript.IsBeingPlayerPushed)
        {
            //here you can add checks specific to player push 

            if (collision.gameObject.tag == "Player")
            {
                return;
            }

            if (collision.gameObject.CompareTag("Star") && collision.gameObject.GetComponent<StarActions>().IsOnPlayer) //so that carrying the star doesn't block the boulder push
            {
                return;
            }
        }

        if (IsBeingPushed)
        {
            //Debug.Log(collision.gameObject);
            StopBoulderPush();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (boulderPlayerPushScript.IsBeingPlayerPushed)
        {
            //here you can add checks specific to player push 

            if (other.gameObject.tag == "Spikes")
            {
                StopBoulderPush();
                return;
            }
        }
    }

    public void InterruptBoulderPush()
    {

    }
}
