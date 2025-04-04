using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderStarPushScript : MonoBehaviour
{
    bool isBeingStarPushed = false;

    [SerializeField] float starPushSpeed = 20f;
    [SerializeField] float pushDistance = 1f;
    [SerializeField] float distanceToCheckForGroundBelowBoulder = 1f;
    [SerializeField] float targetPushDestinationAcceptanceRadius = 0.1f;
    [SerializeField] BoulderSideHitbox[] boulderSideHitboxes = new BoulderSideHitbox[4];

    Rigidbody boulderRigidbody;
    IEnumerator StarPushCoroutine;
    BoulderMoveScript boulderMoveScript;

    void Start()
    {
        boulderRigidbody = GetComponent<Rigidbody>();
        boulderMoveScript = GetComponent<BoulderMoveScript>();
    }

    public void CheckSideHitboxes()
    {
        foreach (BoulderSideHitbox sideHitBox in boulderSideHitboxes)
        {
            if(sideHitBox.WasHitByStar)
            {
                MoveInDirection(sideHitBox.PushDirection, pushDistance);
                break;
            }
        }

        foreach (BoulderSideHitbox sideHitBox in boulderSideHitboxes)
        {
            sideHitBox.Reset();
        }
    }

    public void MoveInDirection(Vector3 direction, float distance)
    {
        if (isBeingStarPushed)
        {
            return;
        }

        //Debug.Log("MoveInDirection");

        StarPushCoroutine = StarPushInDirection(direction, distance);

        RaycastHit hit;


        if (!Physics.Raycast(transform.position + direction * distance, Vector3.down, out hit, distanceToCheckForGroundBelowBoulder)) //to stop boulder from being star pushed into the abyss
        {
            return;
        }

        if (Physics.Raycast(transform.position, direction, out hit, distance)) //to stop boulder from being star pushed into another object
        {
            if (!hit.collider.gameObject.CompareTag("Abyss") && !hit.collider.gameObject.CompareTag("Level Floor") && !hit.collider.gameObject.CompareTag("BoulderSide"))
            {
                //Debug.Log("StarPush Got Interupted by RayCast");
                return;
            }
        }

        StartCoroutine(StarPushCoroutine);
    }

    IEnumerator StarPushInDirection(Vector3 direction, float distance)
    {   
        //repurposed from StarActions "TravelToDestination"

        boulderMoveScript.SnapToFloor();

        Vector3 targetDestination = transform.position + direction * distance;

        //Debug.Log("TRAVELING TO DESTINATION...");
        isBeingStarPushed = true;
        boulderRigidbody.useGravity = false;
        boulderRigidbody.isKinematic = false;


        while (Vector3.Distance(transform.position, targetDestination) > targetPushDestinationAcceptanceRadius)
        {
            //sets velocity to zero as the starthere could sometimes be a downward force (that was not gravity)
            //still unclear where it came from but setting velocity to 0 seems to fix it!
            boulderRigidbody.velocity = new Vector3(0, 0, 0);

            Vector3 tempDirection = targetDestination - transform.position;
            direction = direction.normalized;

            transform.position += tempDirection * starPushSpeed * Time.deltaTime;

            yield return null;
        }

        StopTravelToDestination();
    }

    void StopTravelToDestination()
    {
        StopCoroutine(StarPushCoroutine);
        boulderMoveScript.SnapToFloor();

        isBeingStarPushed = false;
        //boulderRigidbody.useGravity = true;
        boulderRigidbody.isKinematic = true;
        //Debug.Log("Star Push In Direction was STOPPED!");
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
        
        if (isBeingStarPushed && collision.gameObject.tag == "Star")
        {
            return;
        }

        if (isBeingStarPushed)
        {
            //Debug.Log(collision.gameObject);
            StopTravelToDestination();
        }
    }
}
