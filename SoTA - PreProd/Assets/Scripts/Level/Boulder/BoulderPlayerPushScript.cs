using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderPlayerPushScript : MonoBehaviour
{
    [field: Header("Player Push Parameters")]
    [SerializeField] float playerPushSpeed = 10f;
    [SerializeField] float playerPushDistance = 1f;
    [SerializeField] float playerPushCooldown = 0.5f;

    Rigidbody boulderRigidbody;
    BoulderController boulderController;
    BoulderPushController pushController;

    IEnumerator PlayerPushCoroutine;

    private bool isBeingPlayerPushed = false;
    public bool IsBeingPlayerPushed { get { return isBeingPlayerPushed; } }

    void Start()
    {
        boulderRigidbody = GetComponent<Rigidbody>();
        boulderController = GetComponent<BoulderController>();
        pushController = GetComponent<BoulderPushController>();

        //boulderPushSFX = AudioManager.Instance.CreateInstance(FMODEvents.Instance.BoulderSFX);
    }

    public void PlayerPushInDirection(Vector3 direction)
    {
        if (pushController.IsBeingPushed)
        {
            return;
        }

        if (pushController.CheckForValidPushDestination(direction, playerPushDistance))
        {
            PlayerPushCoroutine = PlayerPushInDirection_IEnumerator(direction, playerPushDistance);
            StartCoroutine(PlayerPushCoroutine);
        }
        else
        {
            boulderController.Detach(); //fixes bug where player would start moving boulder in opposite direction when their movement input was held down
        }
    }

    IEnumerator PlayerPushInDirection_IEnumerator(Vector3 direction, float distance)
    {
        //boulderPushSFX.start();

        //repurposed from StarActions "TravelToDestination"

        boulderController.SnapToFloor(); //ensure that boulder is starting the push from a valid position

        Vector3 targetDestination = transform.position + direction * distance;

        isBeingPlayerPushed = true;
        boulderRigidbody.isKinematic = false;

        while (Vector3.Distance(transform.position, targetDestination) > pushController.PushDestinationAcceptanceRadius)
        {
            //sets velocity to zero as there could sometimes be a downward force (that was not gravity)
            //still unclear where it came from but setting velocity to 0 seems to fix it!

            if (!boulderRigidbody.isKinematic) //to avoid warning that sometimes would appear in editor
            {
                boulderRigidbody.velocity = new Vector3(0, 0, 0);
            }

            Vector3 tempDirection = targetDestination - transform.position;
            direction = direction.normalized;

            transform.position += tempDirection * playerPushSpeed * Time.deltaTime;

            yield return null;
        }

        boulderController.SnapToFloor(); //looks weird if this snap only happens AFTER the cooldown

        yield return new WaitForSeconds(playerPushCooldown);

        StopPlayerPush();
    }
    public void StopPlayerPush()
    {
        if (PlayerPushCoroutine != null)
        {
            StopCoroutine(PlayerPushCoroutine);
        }

        isBeingPlayerPushed = false;
        boulderRigidbody.isKinematic = true;
    }
}

