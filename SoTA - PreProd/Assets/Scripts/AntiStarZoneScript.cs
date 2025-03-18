using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiStarZoneScript : MonoBehaviour
{
    [SerializeField] StarActions starActions;
    [SerializeField] Collider collider;
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Star"))
        {
            Debug.Log("Star has been stopped");
            if (starActions.IsOnPlayer == true)
            {
                starActions.CarryToggle();
            }
            starActions.isTraveling = false;
        }
    }
}
