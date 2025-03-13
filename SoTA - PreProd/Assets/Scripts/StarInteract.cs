using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarInteract : MonoBehaviour, IInteractable
{
    // Start is called before the first frame update
    [SerializeField] Transform playerTransform;
    Transform starTransform;
    [SerializeField] bool isOnPlayer;
    [SerializeField] Vector3 onPlayerOffset = new Vector3(0, 3, 0);

    void Start()
    {
        starTransform = gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isOnPlayer)
        {
            starTransform.position = playerTransform.position + onPlayerOffset;
        }
    }

    public void Interact()
    {
        if (isOnPlayer)
        {
            isOnPlayer = false;
        } else if (!isOnPlayer)
        {
            isOnPlayer = true;
        }
    }
}
