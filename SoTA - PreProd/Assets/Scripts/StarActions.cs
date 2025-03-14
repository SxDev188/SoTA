using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StarActions : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform playerTransform;
    Transform starTransform;
    [SerializeField] Rigidbody starRigidbody;
    [SerializeField] float throwSpeed = 10f;
    [SerializeField] float targetDestinationAcceptanceRadius = 0.1f;
    
    [SerializeField] public bool IsOnPlayer { get; private set; }
    [SerializeField] Vector3 onPlayerOffset = new Vector3(0, 3, 0);
    [SerializeField] float frontOfPlayerOffset = 1f;

    void Start()
    {
        starTransform = gameObject.GetComponent<Transform>();
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

    public void Throw(Vector3 targetDestination, Vector3 direction)
    {
        Debug.Log("star was thrown");
        IsOnPlayer = false;
        transform.position = playerTransform.position + frontOfPlayerOffset * direction;

        StartCoroutine(TravelToDestination(targetDestination));
    }

    IEnumerator TravelToDestination(Vector3 targetDestination)
    {
        Debug.Log("TRAVELING TO DESTINATION...");

        while (Vector3.Distance(transform.position, targetDestination) > targetDestinationAcceptanceRadius)
        {
            Vector3 direction = targetDestination - transform.position;
            direction = direction.normalized;

            transform.position += direction * throwSpeed * Time.deltaTime;

            yield return null;
        }

        Debug.Log("DESTINATION REACHED!");
    }
}
