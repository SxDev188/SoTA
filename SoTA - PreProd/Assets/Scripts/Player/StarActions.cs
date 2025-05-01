using System.Collections;
using UnityEditor.UIElements;
using UnityEngine;

public class StarActions : MonoBehaviour
{
    // PUBLIC
    private bool isTraveling = false; //should be made private, where is it used? - goobie
    public bool IsTraveling { get { return isTraveling; } }

    public bool IsOnPlayer 
    { 
        get 
        { 
            return isOnPlayer; 
        } 
        set 
        { 
            isOnPlayer = value;
            if (value)
            {
                starRigidbody.useGravity = false;
            }
            else
            {
                starRigidbody.useGravity = true;
            }
        } 
    }

    // COMPONENTS
    private Transform starTransform;
    private Rigidbody starRigidbody;
    private Transform playerTransform;

    // TWEAKABLE VARIABLES
    [SerializeField] private bool isOnPlayer = false; // Why is this SerializedField? 

    [SerializeField] private float throwSpeed = 10f;
    [SerializeField] private float yOffsetWhenThrown = 0.5f;
    [SerializeField] private float targetDestinationAcceptanceRadius = 0.1f;

    [SerializeField] private float frontOfPlayerOffset = 1f;
    [SerializeField] private Vector3 onPlayerOffset = new Vector3(0, 3, 0);

    // STORING/VALUE VARIABLES
    public IEnumerator TravelCoroutine;
    private float fixedYValueWhenThrown;

    private bool inWall = false;

    void Start()
    {
        starTransform = gameObject.GetComponent<Transform>();
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
        if (isTraveling)
        {
            StopTravelToDestination();
        }

        if (isOnPlayer)
        {
            //drop star sfx here

            isOnPlayer = false;
            starRigidbody.useGravity = true;
        } else if (!isOnPlayer)
        {
            //pick up star sfx here

            isOnPlayer = true;
            starRigidbody.useGravity = false;
        }
    }
    public void Recall()
    {
        if (!isOnPlayer)
        {
            //recall sound effect here

            if(isTraveling)
            {
                StopTravelToDestination();
            }

            isOnPlayer = true;
            starRigidbody.useGravity = false;
        }
    }

    public void Throw(Vector3 targetDestination, Vector3 direction)
    {

        
        if (!inWall)
        {
            //null check here to make star throwable even if savestatemanager is not in scene - Gabbriel
            if (SaveStateManager.Instance != null)
            {
                //Added save here by Linus
                SaveStateManager.Instance.Save();
            }

            isOnPlayer = false;
            Vector3 throwStartPosition = playerTransform.position + frontOfPlayerOffset * direction;

            // Make sure our star is going the right direction?
            fixedYValueWhenThrown = playerTransform.position.y + yOffsetWhenThrown;
            throwStartPosition.y = fixedYValueWhenThrown;

            transform.position = throwStartPosition; // why we do this?

            Vector3 newTargetDestination = targetDestination;
            newTargetDestination.y = fixedYValueWhenThrown;

            TravelCoroutine = TravelToDestination(newTargetDestination);
            StartCoroutine(TravelCoroutine);
        }
        
    }

   public  IEnumerator TravelToDestination(Vector3 targetDestination)
    {
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

   public void TravelOutOfAntiStarZone(Vector3 position)
    {
        TravelCoroutine = TravelToDestination(position);
        StartCoroutine(TravelCoroutine);      
    }

    public void StopTravelToDestination()
    {
        StopCoroutine(TravelCoroutine);

        isTraveling = false;
        starRigidbody.useGravity = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Abyss"))
        {
            SaveStateManager.Instance.Load();
        }

        if (other.gameObject.tag == "StarPickupTrigger" && !isOnPlayer && !isTraveling)
        {
            isOnPlayer = true;
            starRigidbody.useGravity = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (isOnPlayer)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("StopStar"))
            {
                inWall = false;
            }
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (isOnPlayer)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("StopStar"))
            {
                inWall = true;
            }
        }
        if (collision.gameObject.tag == "Player")
        {
            //if (isTraveling)
            //{
            //    return;
            //}
            return;
        }

        if (collision.gameObject.tag == "Spikes" )
        {
            return;
        }
        
        if (collision.gameObject.tag == "PressurePlate" )
        {
            return;
        }

        if (collision.gameObject.tag == "Button" && isTraveling)
        {
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
            StopTravelToDestination();
        }
    }
}
