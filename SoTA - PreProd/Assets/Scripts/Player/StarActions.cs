using FMOD.Studio;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class 
    StarActions : MonoBehaviour
{
    // PUBLIC
    private bool isTraveling = false; //should be made private, where is it used? - goobie // is checked in AntiStarZoneScript in the logic for pushing out the star, but separating isTraveling and IsTraveling into two different bools works aswell :) -Emil
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
    [SerializeField] private float dropStarCooldownDuration = 0.5f;


    // STORING/VALUE VARIABLES
    public IEnumerator TravelCoroutine;
    private float fixedYValueWhenThrown;
    private EventInstance starThrowSFX;
    private bool canBePickedUp = true;
    private CooldownTimer dropStarCooldown;




    // For SFX
    bool isFallingAndHasNotLanded = true;

    void Start()
    {
        starTransform = gameObject.GetComponent<Transform>();
        starRigidbody = gameObject.GetComponent<Rigidbody>();
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        starThrowSFX = AudioManager.Instance.CreateInstance(FMODEvents.Instance.StarThrowSFX);
        dropStarCooldown = new CooldownTimer(this);
    }

    void Update()
    {
        if (isOnPlayer)
        {
            starTransform.position = playerTransform.position + onPlayerOffset;
        }

        if (!isTraveling)
        {
            starThrowSFX.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }

    public void CarryToggle()
    {
        if (isTraveling)
        {
            StopTravelToDestination(false);
        }

        if (isOnPlayer)
        {
            Drop();
        } else if (!isOnPlayer)
        {
            Pickup();
        }
    }

    private void Pickup()
    {
        if(!canBePickedUp)
        {
            return;
        }

        //pick up star sfx here
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.StarPickupSFX);
        isOnPlayer = true;
        starRigidbody.useGravity = false;
    }

    private void Drop()
    {
        //drop star sfx here

        isOnPlayer = false;
        starRigidbody.useGravity = true;

        isFallingAndHasNotLanded = true;
        SaveStateManager.Instance.Save();

        canBePickedUp = false;
        dropStarCooldown.Start(dropStarCooldownDuration, () => { canBePickedUp = true; });
    }
    public void Recall()
    {
        if (!isOnPlayer)
        {
            //recall sound effect here

            if(isTraveling)
            {
                StopTravelToDestination(false);
            }

            isOnPlayer = true;
            starRigidbody.useGravity = false;
        }
    }

    public void Throw(Vector3 targetDestination, Vector3 direction)
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

        //soun deffect was moved to PlayerStarActionController since thats where we can determine whether or not it is normal or strong throw
        //AudioManager.Instance.PlayOneShot(FMODEvents.Instance.StarThrowAttackSFX); 
    }

    public void TravelOutOfAntiStarZone(Vector3 targetDestination)
    {
        TravelCoroutine = TravelToDestination(targetDestination);
        StartCoroutine(TravelCoroutine);
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

        StopTravelToDestination(false);
    }

    public void StopTravelToDestination(bool isColliding)
    {
        StopCoroutine(TravelCoroutine);

        isTraveling = false;
        starRigidbody.useGravity = true;

        //SFX
        starThrowSFX.setParameterByNameWithLabel("StarThrowState", isColliding ? "Colliding" : "Landing");
        isFallingAndHasNotLanded = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Abyss"))
        {
            if (!isOnPlayer) //if isOnPlayer is true, then player will also collide with abyss, which leads to death --> load save state
            {
                Recall(); //I put recall here instead of load so - Goobie
                //SaveStateManager.Instance.Load();
            }
        }

        if (other.gameObject.tag == "StarPickupTrigger" && !isOnPlayer && !isTraveling && !playerTransform.GetComponent<PlayerStarActionController>().IsBeingGravityPulled)
        {
            //isOnPlayer = true;
            //starRigidbody.useGravity = false;
            Pickup();
        }
    }


    void OnCollisionEnter(Collision collision)
    {

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
            starThrowSFX.setParameterByNameWithLabel("StarThrowState", "Colliding_Interactable");
            StopTravelToDestination(true);
            return;
        }

        if (collision.gameObject.tag == "Lamp" && isTraveling)
        {
            collision.gameObject.GetComponent<LampScript>().Interact();
            starThrowSFX.setParameterByNameWithLabel("StarThrowState", "Colliding_Interactable");
            StopTravelToDestination(true);
            return;
        }

        if (!isOnPlayer && isFallingAndHasNotLanded && collision.gameObject.CompareTag("Level Floor"))
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.StarLandFloorSFX, Vector3.zero);
            isFallingAndHasNotLanded = false;
        }

        if (isTraveling)
        {
            StopTravelToDestination(true);
            starThrowSFX.setParameterByNameWithLabel("StarThrowState", "Colliding_Regular");
        }
    }
}
