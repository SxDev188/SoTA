using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public class BoulderMoveScript : MonoBehaviour
{
    [SerializeField] private float interactionRange = 2f;

    private bool isMoving = false;
    private GameObject player;
    private PlayerController playerController;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    private void Update()
    {
        // TODO: Implement with input system
        if (Input.GetKeyDown(KeyCode.E) 
            && PlayerIsClose() 
            && playerController.RayBoulderInteration(interactionRange) != Vector3.zero)
        {
            isMoving = !isMoving;
        }
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            this.transform.SetParent(player.transform);
        }
        else
        {
            SnapToFloor();
            this.transform.parent = null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag != "Level Floor" || collision.gameObject.tag != "Player")
        {
            isMoving = false;
        }
    }

    private void SnapToFloor()
    {
        if (!isMoving)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1f))
            {
                transform.position = new Vector3(hit.transform.position.x, transform.position.y, hit.transform.position.z);
            }
        }
    }

    private bool PlayerIsClose()
    {
        return Vector3.Distance(transform.position, player.transform.position) <= interactionRange;
    }

}
