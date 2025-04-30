using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarParticles : MonoBehaviour
{
    [SerializeField] PlayerStarActionController playerStarActionController;
    [SerializeField] Transform playerTransform;
    [SerializeField] StarActions starActions;
    private float gravityPullRange;
    private float recallRange;
    private ParticleSystem gravityPullParticles;
    private ParticleSystem recallParticles;


    // Start is called before the first frame update
    void Start()
    {
        gravityPullRange = playerStarActionController.GravityPullRange;
        recallRange = playerStarActionController.RecallRange;

        var particleSystems = GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in particleSystems)
        {
            if (ps.gameObject.name.Contains("Gravity", System.StringComparison.OrdinalIgnoreCase))
                gravityPullParticles = ps;
            else if (ps.gameObject.name.Contains("Recall", System.StringComparison.OrdinalIgnoreCase))
                recallParticles = ps;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (!starActions.IsOnPlayer)
        {
            // Gravity pull logic
            if (distanceToPlayer <= gravityPullRange)
            {
                if (gravityPullParticles && !gravityPullParticles.isPlaying)
                    gravityPullParticles.Play();
            }
            else
            {
                if (gravityPullParticles && gravityPullParticles.isPlaying)
                    gravityPullParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }

            // Recall logic
            if (distanceToPlayer <= recallRange)
            {
                if (recallParticles && !recallParticles.isPlaying)
                    recallParticles.Play();
            }
            else
            {
                if (recallParticles && recallParticles.isPlaying)
                    recallParticles.Stop();
            }
        }
        else
        {
            recallParticles.Stop();
            gravityPullParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}
