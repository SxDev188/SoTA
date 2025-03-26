using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StarActions : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform playerTransform;
    Transform starTransform;
    SphereCollider starCollider;

    [SerializeField] Rigidbody starRigidbody;
    [SerializeField] float throwSpeed = 10f;
    [SerializeField] float targetDestinationAcceptanceRadius = 0.1f;
    
    [SerializeField] public bool IsOnPlayer { get; private set; }
    [SerializeField] Vector3 onPlayerOffset = new Vector3(0, 3, 0);
    [SerializeField] float frontOfPlayerOffset = 1f;
    public bool isTraveling = false;

    IEnumerator TravelCoroutine;

    void Start()
    {
        starTransform = gameObject.GetComponent<Transform>();
        starCollider = gameObject.GetComponent<SphereCollider>();
    }

    void Update()
    {
        if (IsOnPlayer)
        {
            starTransform.position = playerTransform.position + onPlayerOffset;
        }
    }

    public void CarryToggle()
    {
        if (IsOnPlayer)
        {
            IsOnPlayer = false;
        } else if (!IsOnPlayer)
        {
            IsOnPlayer = true;
        }
    }
    public void Recall()
    {
        if (!IsOnPlayer)
        {
            IsOnPlayer = true;
        }
    }

    public void Throw(Vector3 targetDestination, Vector3 direction)
    {
        Debug.Log("star was thrown");
        IsOnPlayer = false;
        transform.position = playerTransform.position + frontOfPlayerOffset * direction;
        
        TravelCoroutine = TravelToDestination(targetDestination);
        StartCoroutine(TravelCoroutine);
    }

    IEnumerator TravelToDestination(Vector3 targetDestination)
    {
        Debug.Log("TRAVELING TO DESTINATION...");
        isTraveling = true;
        starRigidbody.useGravity = false;

        while (Vector3.Distance(transform.position, targetDestination) > targetDestinationAcceptanceRadius)
        {
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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" )
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

        if(collision.gameObject.tag == "Lamp" && isTraveling)
        {
            LampScript lamp = collision.gameObject.GetComponent<LampScript>();
            if (!lamp.IsLit)
            {
                lamp.Activate();
            } else if(lamp.IsLit)
            {
                lamp.Deactivate();
            }
        }

        if (isTraveling)
        {
            StopTravelToDestination();
        }
    }
}
