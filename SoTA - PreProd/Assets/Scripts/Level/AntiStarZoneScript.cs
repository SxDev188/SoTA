using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiStarZoneScript : MonoBehaviour, IActivatable
{
    StarActions starActions;

    void Start()
    {
        // Fetch the star in the scene
        starActions = GameObject.FindGameObjectWithTag("Star").GetComponent<StarActions>();
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

    public void Activate()
    {
        gameObject.SetActive(false);
    }

    public void Deactivate()
    {
        gameObject.SetActive(true);
    }
}
