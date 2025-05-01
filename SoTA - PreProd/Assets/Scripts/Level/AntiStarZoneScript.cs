using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AntiStarZoneScript : MonoBehaviour, IActivatable
{
    public StarActions starActions;
    public PlayerStarActionController playerStarActionController;
    ParticleSystemRenderer particleSystemRenderer;
    MeshRenderer parentMeshRenderer;
    Color color;
    [SerializeField] int EjectStarX;
    [SerializeField] int EjectStarZ;

    void Start()
    {
        // Fetch the star in the scene
        starActions = GameObject.FindGameObjectWithTag("Star").GetComponent<StarActions>();
        playerStarActionController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStarActionController>();
        particleSystemRenderer = GetComponent<ParticleSystemRenderer>();
        parentMeshRenderer = GetComponentInParent<MeshRenderer>();
        //make the color match that of the top of the tile
        color = parentMeshRenderer.materials[4].color;
        particleSystemRenderer.material.color = color;

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerStarActionController.DisallowStarOnPlayer();
        }
        if (other.CompareTag("Star"))
        {
            if (starActions.IsOnPlayer == true)
            {
                starActions.CarryToggle();
            }
            starActions.StopTravelToDestination();
            Vector3 dir = other.transform.position + (other.transform.position - transform.position);
            if (!starActions.isTraveling)
            {
                starActions.TravelOutOfAntiStarZone(new Vector3(dir.x, playerStarActionController.transform.position.y, dir.z));

            }

        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerStarActionController.DisallowStarOnPlayer();
        }
        if (other.CompareTag("Star"))
        {
            if (starActions.IsOnPlayer == true)
            {
                starActions.CarryToggle();
            }

            if (!starActions.isTraveling)
            {
                starActions.TravelOutOfAntiStarZone(new Vector3(EjectStarX*100, playerStarActionController.transform.position.y, EjectStarZ*100));
            }

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        { 
            playerStarActionController.AllowStarOnPlayer();
        
        }
        if (other.CompareTag("Star"))
        {
            if (starActions.isTraveling)
            {
                starActions.StopTravelToDestination();
            }

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
