using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEngine;

public class BoulderStarPushScript : MonoBehaviour
{
    bool isBeingStarPushed = false;
    public bool IsBeingStarPushed { get { return isBeingStarPushed; } }

    [SerializeField] float starPushSpeed = 20f;
    [SerializeField] float starPushDistance = 1f;
    [SerializeField] float starPushCooldown = 0.2f;


    [SerializeField] float playerPushSpeed = 10f;
    [SerializeField] float playerPushDistance = 1f;
    [SerializeField] float playerPushCooldown = 0.5f;
    
    [SerializeField] float distanceToCheckForGroundBelowBoulder = 1f;
    [SerializeField] float targetPushDestinationAcceptanceRadius = 0.01f;
    [SerializeField] BoulderSideHitbox[] boulderSideHitboxes = new BoulderSideHitbox[4];

    Rigidbody boulderRigidbody;
    IEnumerator StarPushCoroutine;
    IEnumerator PlayerPushCoroutine;
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
                StarPushInDirection(sideHitBox.PushDirection, starPushDistance);
                break;
            }
        }

        foreach (BoulderSideHitbox sideHitBox in boulderSideHitboxes)
        {
            sideHitBox.Reset();
        }
    }

    public void StarPushInDirection(Vector3 direction, float distance)
    {
        if (isBeingStarPushed)
        {
            return;
        }

        //Debug.Log("MoveInDirection");

        StarPushCoroutine = StarPushInDirection_IEnumerator(direction, distance);

        if (CheckForValidPushDestination(direction, distance))
        {
            StartCoroutine(StarPushCoroutine);
        }
    }

    IEnumerator StarPushInDirection_IEnumerator(Vector3 direction, float distance)
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
        yield return new WaitForSeconds(starPushCooldown);


        StopPushInDirection();
    }

    private bool CheckForValidPushDestination(Vector3 direction, float distance)
    {
        RaycastHit hit;

        if (!Physics.Raycast(transform.position + direction * distance, Vector3.down, out hit, distanceToCheckForGroundBelowBoulder)) //to stop boulder from being star pushed into the abyss
        {
            return false;
        }

        if (Physics.Raycast(transform.position, direction, out hit, distance)) //to stop boulder from being star pushed into another object
        {
            if (!hit.collider.gameObject.CompareTag("Abyss") && !hit.collider.gameObject.CompareTag("Level Floor") && !hit.collider.gameObject.CompareTag("BoulderSide"))
            {
                //Debug.Log("StarPush Got Interupted by RayCast");
                return false;
            }
        }

        return true;
    }
    void StopPushInDirection()
    {
        if (StarPushCoroutine != null)
        {
            StopCoroutine(StarPushCoroutine);
        }
        
        if (PlayerPushCoroutine != null)
        {
            StopCoroutine(PlayerPushCoroutine);
        }

        isBeingStarPushed = false;
        //boulderRigidbody.useGravity = true;
        boulderRigidbody.isKinematic = true;
        //Debug.Log("Star Push In Direction was STOPPED!");
        boulderMoveScript.SnapToFloor();
    }

    public void PlayerPushInDirection(Vector3 direction, float distance)
    {
        if (isBeingStarPushed)
        {
            return;
        }

        //Debug.Log("MoveInDirection");

        if (CheckForValidPushDestination(direction, distance))
        {
            PlayerPushCoroutine = PlayerPushInDirection_IEnumerator(direction, distance);
            StartCoroutine(PlayerPushCoroutine);
        } else
        {
            boulderMoveScript.Detach();
        }
    }

    IEnumerator PlayerPushInDirection_IEnumerator(Vector3 direction, float distance)
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

            transform.position += tempDirection * playerPushSpeed * Time.deltaTime;

            yield return null;
        }
        boulderMoveScript.SnapToFloor();

        yield return new WaitForSeconds(playerPushCooldown);

        StopPushInDirection();
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
        
        if (isBeingStarPushed && collision.gameObject.tag == "Player")
        {
            return;
        }

        if (isBeingStarPushed)
        {
            //Debug.Log(collision.gameObject);
            StopPushInDirection();
        }
    }
}
