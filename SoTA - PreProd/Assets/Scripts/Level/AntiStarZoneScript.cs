using UnityEngine;

public class AntiStarZoneScript : MonoBehaviour, IActivatable
{
    public StarActions starActions;
    public PlayerStarActionController playerStarActionController;
    ParticleSystemRenderer particleSystemRenderer;
    MeshRenderer parentMeshRenderer;
    Color color;

    void Start()
    {
        // Fetch the star in the scene
        starActions = GameObject.FindGameObjectWithTag("Star").GetComponent<StarActions>();
        particleSystemRenderer = GetComponent<ParticleSystemRenderer>();
        parentMeshRenderer = GetComponentInParent<MeshRenderer>();
        color = parentMeshRenderer.materials[4].color;
        particleSystemRenderer.material.color = color;

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Star"))
        {
            if (starActions.IsOnPlayer == true)
            {
                starActions.CarryToggle();
            } 
            starActions.StopTravelToDestination();
        }
        if (other.CompareTag("Player"))
        {
            playerStarActionController.DisallowStarOnPlayer();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        { 
            playerStarActionController.AllowStarOnPlayer();
        
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
