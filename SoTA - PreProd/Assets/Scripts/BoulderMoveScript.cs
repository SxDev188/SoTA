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

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && PlayerIsClose())
        {
            isMoving = !isMoving;
        }
    }

    private void FixedUpdate()
    {

        // Snap to floor
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1f))
        {
            if (!isMoving)
            {
                transform.position = new Vector3(hit.transform.position.x, transform.position.y, hit.transform.position.z);
            }
        }

        if (isMoving)
        {
            this.transform.SetParent(player.transform);
        }
        else
        {
            this.transform.parent = null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Level Object")
        {
            isMoving = false;
        }
    }

    private bool PlayerIsClose()
    {
        return Vector3.Distance(transform.position, player.transform.position) <= interactionRange;
    }

}
