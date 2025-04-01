using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StarActions : MonoBehaviour
{
    Transform playerTransform;
    Transform starTransform;
    SphereCollider starCollider;

    Rigidbody starRigidbody;
    [SerializeField] float throwSpeed = 10f;
    [SerializeField] float targetDestinationAcceptanceRadius = 0.1f;
    
    public bool IsOnPlayer { get { return isOnPlayer; } set { isOnPlayer = value; } }
    [SerializeField] private bool isOnPlayer = false;
    [SerializeField] Vector3 onPlayerOffset = new Vector3(0, 3, 0);
    [SerializeField] float frontOfPlayerOffset = 1f;

    [SerializeField] float yOffsetWhenThrown = 0.5f;
    float fixedYValueWhenThrown;
    
    public bool isTraveling = false;

    IEnumerator TravelCoroutine;

    void Start()
    {
        starTransform = gameObject.GetComponent<Transform>();
        starCollider = gameObject.GetComponent<SphereCollider>();
        starRigidbody = gameObject.GetComponent<Rigidbody>();
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    void Update()
    {
        if (isOnPlayer)
        {
            starTransform.position = playerTransform.position + onPlayerOffset;
        }
    }

    public void CarryToggle()
    {
        if (isOnPlayer)
        {
            isOnPlayer = false;
        } else if (!isOnPlayer)
        {
            isOnPlayer = true;
        }
    }
    public void Recall()
    {
        if (!isOnPlayer)
        {
            isOnPlayer = true;
        }
    }

    public void Throw(Vector3 targetDestination, Vector3 direction)
    {
        Debug.Log("star was thrown");

        //null check here to make star throwable even if savestatemanager is not in scene - Gabbriel
        if (SaveStateManager.Instance != null)
        {
            //Added save here by Linus
            SaveStateManager.Instance.Save();
        }

        isOnPlayer = false;
        Vector3 throwStartPosition = playerTransform.position + frontOfPlayerOffset * direction;
        
        fixedYValueWhenThrown = playerTransform.position.y + yOffsetWhenThrown;
        throwStartPosition.y = fixedYValueWhenThrown;

        transform.position = throwStartPosition;

        Vector3 newTargetDestination = targetDestination;
        newTargetDestination.y = fixedYValueWhenThrown;

        TravelCoroutine = TravelToDestination(newTargetDestination);
        StartCoroutine(TravelCoroutine);
    }

    IEnumerator TravelToDestination(Vector3 targetDestination)
    {
        Debug.Log("TRAVELING TO DESTINATION...");
        isTraveling = true;
        starRigidbody.useGravity = false;


        while (Vector3.Distance(transform.position, targetDestination) > targetDestinationAcceptanceRadius)
        {
            //sets velocity to zero as the star SOMEHOW got some downward force (that was not gravity) related to the player rigidbody
            //still unclear where it came from but setting velocity to 0 seems to fix it!
            starRigidbody.velocity = new Vector3(0, 0, 0);

            Vector3 direction = targetDestination - transform.position;
            direction = direction.normalized;

            transform.position += direction * throwSpeed * Time.deltaTime;

            yield return null;
        }

        StopTravelToDestination();
    }

    void StopTravelToDestination()
    {
        StopCoroutine(TravelCoroutine);

        isTraveling = false;
        starRigidbody.useGravity = true;
        Debug.Log("TRAVEL TO DESTINATION WAS STOPPED!");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Abyss"))
        {
            isOnPlayer = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Player")
        {
            return;
        }

        if (collision.gameObject.tag == "Spikes" )
        {
            return;
        }

        if (collision.gameObject.tag == "Button" && isTraveling)
        {
            //collision.gameObject.CollisionLogicMethod();
            //here you can call method in whatever signaler object you collide with (such as a button)
            collision.gameObject.GetComponent<ButtonScript>().Interact();
            StopTravelToDestination();
        }

        if (collision.gameObject.tag == "Lamp" && isTraveling)
        {
            collision.gameObject.GetComponent<LampScript>().Interact();
            StopTravelToDestination();
        }

        if (isTraveling)
        {
            //Debug.Log(collision.gameObject);
            StopTravelToDestination();
        }
    }
}
