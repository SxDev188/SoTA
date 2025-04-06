using UnityEngine;

public class BoulderSideHitbox : MonoBehaviour
{
    [SerializeField] private Vector3 pushDirection;

    public bool WasHitByStar { get; private set; }
    public Vector3 PushDirection { get { return pushDirection; } }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Star"))
        {
            StarActions starActions = other.gameObject.GetComponent<StarActions>();

            if (starActions.IsOnPlayer)
            {
                return;
            }
            else if(starActions.isTraveling)
            {
                starActions.StopTravelToDestination();
                WasHitByStar = true;
                this.GetComponentInParent<BoulderStarPushScript>().CheckSideHitboxes();
            }
        }

    }

    public void Reset()
    {
        WasHitByStar = false;
    }
}
