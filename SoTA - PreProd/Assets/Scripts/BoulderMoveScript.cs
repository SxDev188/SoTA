using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoulderMoveScript : MonoBehaviour
{
    private bool isMoving = false;

    private void Start()
    {

    }

    private void FixedUpdate()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Player")
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        if (isMoving)
        {
            this.transform.SetParent(collision.transform);
            Physics.IgnoreCollision(this.GetComponent<Collider>(), collision.collider);
        }

    }


}
