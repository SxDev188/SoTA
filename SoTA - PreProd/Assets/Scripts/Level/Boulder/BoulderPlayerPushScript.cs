using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoulderPlayerPushScript : MonoBehaviour
{
    //this script handles moving the boulder when player pushes it or pulls on it

    [field: Header("Player Push Parameters")]
    [SerializeField] float playerPushSpeed = 10f;
    [SerializeField] float playerPushDistance = 1f;
    [SerializeField] float playerPushCooldown = 0.5f;

    Rigidbody boulderRigidbody;
    BoulderController boulderController;
    BoulderPushController pushController;
    PlayerController playerController;

    IEnumerator PlayerPushCoroutine;

    private bool isBeingPlayerPushed = false;
    public bool IsBeingPlayerPushed { get { return isBeingPlayerPushed; } }

    void Start()
    {
        boulderRigidbody = GetComponent<Rigidbody>();
        boulderController = GetComponent<BoulderController>();
        pushController = GetComponent<BoulderPushController>();

        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        //boulderPushSFX = AudioManager.Instance.CreateInstance(FMODEvents.Instance.BoulderSFX);
    }

    public void PlayerPushInDirection(Vector3 direction)
    {
        if (pushController.IsBeingPushed)
        {
            return;
        }

        if (pushController.CheckForValidPushDestination(direction, playerPushDistance) && CheckValidPlayerDestinationAfterPush(direction, playerPushDistance))
        {
            PlayerPushCoroutine = PlayerPushInDirection_IEnumerator(direction, playerPushDistance);
            StartCoroutine(PlayerPushCoroutine);
        }
        else
        {
            boulderController.Detach(); //fixes bug where player would start moving boulder in opposite direction when their movement input was held down
        }
    }

    public bool CheckValidPlayerDestinationAfterPush(Vector3 direction, float distance)
    {
        //I know this is super nested but atm this is what I can do - Goobie
        RaycastHit hit;

        Debug.DrawRay(playerController.transform.position, direction, Color.red, 1.0f);

        if (Physics.Raycast(playerController.transform.position, direction, out hit, distance))
        {
            if (!hit.collider.gameObject.CompareTag("PressurePlate") && !hit.collider.gameObject.CompareTag("AntiStarZone") && !hit.collider.gameObject.CompareTag("CameraPan") && !hit.collider.gameObject.CompareTag("Spikes"))
            {
                if (hit.collider.gameObject.CompareTag("BoulderSide"))
                {
                    if (hit.collider.gameObject.GetComponentInParent<BoulderController>() != BoulderController.GetCurrentlyActiveBoulder())
                    {
                        return false;
                    } else
                    {
                        return true;
                    }
                }
                
                if (hit.collider.gameObject.CompareTag("Boulder"))
                {
                    if (hit.collider.gameObject.GetComponent<BoulderController>() != BoulderController.GetCurrentlyActiveBoulder())
                    {
                        return false;
                    } else
                    {
                        return true;
                    }
                }

                Debug.Log("RAYCAST HIT SOMETHING WITH TAG: " + hit.collider.gameObject.tag);

                return false;
            }
        }

        return true;
    }

    IEnumerator PlayerPushInDirection_IEnumerator(Vector3 direction, float distance)
    {
        //boulderPushSFX.start();
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.BoulderSFX);

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

            transform.position += tempDirection * playerPushSpeed * Time.deltaTime;

            yield return null;
        }

        boulderController.SnapToFloor(); //looks weird if this snap happens AFTER the cooldown

        if (!playerController.IsGrounded() && boulderController.IsAttached)
        {
            boulderController.Detach();
        }

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

