using UnityEngine;

public class BoulderSideHitbox : MonoBehaviour
{
    [SerializeField] private Vector3 pushDirection;

    private bool blocked = false;
    private int amountInWay = 0;
    public bool Blocked { get { return blocked; } }
    public bool WasHitByStar { get; private set; }
    public Vector3 PushDirection { get { return pushDirection; } }


    private void OnTriggerExit(Collider other)
    {
        if(!other.CompareTag("Star") && !other.CompareTag("Player") && !other.CompareTag("PressurePlate") && !other.CompareTag("EndPoint"))
        {
            amountInWay--;
            if (amountInWay == 0)
            {
                blocked = false;
                Debug.Log(-pushDirection + " is Unblocked");
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Star") && !other.CompareTag("Player") && !other.CompareTag("PressurePlate") && !other.CompareTag("EndPoint"))
        {
            amountInWay++;
            blocked = true;
            Debug.Log(-pushDirection + " is Blocked by " + other.name);
        }

        if (other.gameObject.CompareTag("Star"))
        {
            StarActions starActions = other.gameObject.GetComponent<StarActions>();

            if (starActions.IsOnPlayer)
            {
                return;
            }
            else if(starActions.IsTraveling)
            {
                starActions.StopTravelToDestination(true);
                WasHitByStar = true;
                this.GetComponentInParent<BoulderStarPushScript>().CheckSideHitboxes();
            }
        }

    }
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Star"))
    //    {
    //        StarActions starActions = other.gameObject.GetComponent<StarActions>();

    //        if (starActions.IsOnPlayer)
    //        {
    //            return;
    //        }
    //        else if (starActions.IsTraveling)
    //        {
    //            starActions.StopTravelToDestination(true);
    //            WasHitByStar = true;
    //            this.GetComponentInParent<BoulderStarPushScript>().CheckSideHitboxes();
    //        }
    //    }

    //}

    public void Reset()
    {
        WasHitByStar = false;
    }
}
