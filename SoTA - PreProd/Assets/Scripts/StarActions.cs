using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarActions : MonoBehaviour, IInteractable
{
    // Start is called before the first frame update
    [SerializeField] Transform playerTransform;
    Transform starTransform;
    [SerializeField] Rigidbody starRigidbody;
    [SerializeField] public bool IsOnPlayer { get; private set; }
    [SerializeField] Vector3 onPlayerOffset = new Vector3(0, 3, 0);


    Vector3 throwTargetPosition;

    void Start()
    {
        starTransform = gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOnPlayer)
        {
            starTransform.position = playerTransform.position + onPlayerOffset;
        }
    }

    public void Interact()
    {
        //is essentially a pick up/drop toggle

        if (IsOnPlayer)
        {
            IsOnPlayer = false;
        } else if (!IsOnPlayer)
        {
            IsOnPlayer = true;
        }
    }

    public void Throw(Vector3 targetDestination)
    {
        IsOnPlayer = false;
        Debug.Log("star was thrown");
        starRigidbody.AddForce(targetDestination);
    }
}
