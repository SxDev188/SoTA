using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiStarZoneScript : MonoBehaviour
{
    [SerializeField] StarActions starActions;

    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Star"))
        {
            if (starActions.IsOnPlayer == true)
            {
                starActions.CarryToggle();
            }
            starActions.isTraveling = false;
        }
    }
}
