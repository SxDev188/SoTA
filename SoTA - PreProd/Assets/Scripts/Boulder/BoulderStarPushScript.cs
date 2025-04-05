using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class BoulderStarPushScript : MonoBehaviour
{
    //this script is a bit too big. it takes care of both star push and player push.
    //they should probably be separated in the future but for now they use so many of the same methods/variables that they are both in this script

    [field: Header("Star Push Parameters")]

    [SerializeField] float starPushSpeed = 20f;
    [SerializeField] float starPushDistance = 1f;
    [SerializeField] float starPushCooldown = 0.2f;

    [field: Header("Player Push Parameters")]
    [SerializeField] float playerPushSpeed = 10f;
    [SerializeField] float playerPushDistance = 1f;
    [SerializeField] float playerPushCooldown = 0.5f;

    [SerializeField] bool debugMode = false; //shows the raycast checking for valid push position
    [SerializeField] Vector3 raycastOffset = new Vector3(0, -0.4f, 0); //offsets the raycast checking for valid push position, without this it misses the spikes capsule collider as it is too low to the ground

    [SerializeField] float distanceToCheckForGroundBelowBoulder = 1f;
    [SerializeField] float targetPushDestinationAcceptanceRadius = 0.01f;
    [SerializeField] BoulderSideHitbox[] boulderSideHitboxes = new BoulderSideHitbox[4];

    Rigidbody boulderRigidbody;
    BoulderMoveScript boulderMoveScript;

    IEnumerator StarPushCoroutine;
    IEnumerator PlayerPushCoroutine;

    private bool isBeingStarPushed = false;
    private bool isBeingPlayerPushed = false;
    public bool IsBeingPushed
    {
        get {
            if (isBeingStarPushed || isBeingPlayerPushed)
                return true;
            else
                return false;
        }
    }

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
        if (IsBeingPushed)
        {
            return;
        }

        StarPushCoroutine = StarPushInDirection_IEnumerator(direction, distance);

        if (CheckForValidPushDestination(direction, distance))
        {
            StartCoroutine(StarPushCoroutine);
        }
    }

    private bool CheckForValidPushDestination(Vector3 direction, float distance)
    {
        RaycastHit hit;

        if(debugMode)
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

            if (!hit.collider.gameObject.CompareTag("Abyss") && !hit.collider.gameObject.CompareTag("Level Floor") && !hit.collider.gameObject.CompareTag("BoulderSide") && !hit.collider.gameObject.CompareTag("PressurePlate") && !hit.collider.gameObject.CompareTag("Player"))
            {
                //add tags here that you want boulder to ignore, but remember to also add them in the OnCollisionEnter check

                Debug.Log("RAYCAST HIT SOMETHING WITH TAG: " + hit.collider.gameObject.tag);

                return false;
            }
        }

        return true;
    }

    IEnumerator StarPushInDirection_IEnumerator(Vector3 direction, float distance)
    {
        //repurposed from StarActions "TravelToDestination"

        boulderMoveScript.SnapToFloor(); //ensure that boulder is starting the push from a valid position

        Vector3 targetDestination = transform.position + direction * distance;

        isBeingStarPushed = true;
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

        boulderMoveScript.SnapToFloor(); //looks weird if this snap only happens AFTER the cooldown

        yield return new WaitForSeconds(starPushCooldown);

        StopPushInDirection();
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
        isBeingPlayerPushed = false;
        boulderRigidbody.isKinematic = true;

        boulderMoveScript.SnapToFloor(); //probably not necessary, but better safe than sorry
    }

    public void PlayerPushInDirection(Vector3 direction)
    {
        if (IsBeingPushed)
        {
            return;
        }

        if (CheckForValidPushDestination(direction, playerPushDistance))
        {
            PlayerPushCoroutine = PlayerPushInDirection_IEnumerator(direction, playerPushDistance);
            StartCoroutine(PlayerPushCoroutine);
        } else
        {
            boulderMoveScript.Detach(); //fixes bug where player would start moving boulder in opposite direction when their movement input was held down
        }
    }

    IEnumerator PlayerPushInDirection_IEnumerator(Vector3 direction, float distance)
    {
        //repurposed from StarActions "TravelToDestination"

        boulderMoveScript.SnapToFloor(); //ensure that boulder is starting the push from a valid position

        Vector3 targetDestination = transform.position + direction * distance;

        isBeingPlayerPushed = true;
        boulderRigidbody.isKinematic = false;

        while (Vector3.Distance(transform.position, targetDestination) > targetPushDestinationAcceptanceRadius)
        {
            //sets velocity to zero as there could sometimes be a downward force (that was not gravity)
            //still unclear where it came from but setting velocity to 0 seems to fix it!
            boulderRigidbody.velocity = new Vector3(0, 0, 0);

            Vector3 tempDirection = targetDestination - transform.position;
            direction = direction.normalized;

            transform.position += tempDirection * playerPushSpeed * Time.deltaTime;

            yield return null;
        }

        boulderMoveScript.SnapToFloor(); //looks weird if this snap only happens AFTER the cooldown

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
        
        if (isBeingStarPushed)
        {
            //here you can add checks specific to star push 

            if (collision.gameObject.tag == "Star")
            {
                return;
            }
        }

        if (isBeingPlayerPushed)
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
            StopPushInDirection();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isBeingPlayerPushed)
        {
            //here you can add checks specific to player push 

            if (other.gameObject.tag == "Spikes")
            {
                StopPushInDirection();
                return;
            }
        }
    }
}
