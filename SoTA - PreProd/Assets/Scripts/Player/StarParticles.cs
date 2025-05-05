using FMOD.Studio;
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
    private ParticleSystem trailParticles;

    private EventInstance starShimmerSFX;

    void Start()
    {
        playerStarActionController = FindObjectOfType<PlayerStarActionController>();
        playerTransform = GameObject.FindWithTag("Player").transform;
        starActions = GetComponent<StarActions>();

        gravityPullRange = playerStarActionController.GravityPullRange;
        recallRange = playerStarActionController.RecallRange;

        var particleSystems = GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in particleSystems)
        {
            if (ps.gameObject.name.Contains("Gravity", System.StringComparison.OrdinalIgnoreCase))
                gravityPullParticles = ps;
            else if (ps.gameObject.name.Contains("Recall", System.StringComparison.OrdinalIgnoreCase))
                recallParticles = ps;
            else if (ps.gameObject.name.Contains("Trail", System.StringComparison.OrdinalIgnoreCase))
                    trailParticles = ps;
        }

        starShimmerSFX = AudioManager.Instance.CreateInstance(FMODEvents.Instance.StarShimmerSFX);
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (!starActions.IsOnPlayer) // Applies the particle effects only if the Star isn't held by player
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
        else // If player is holding Star, the gravityPull particles get stopped and cleared but recallParticles continue
        {
            gravityPullParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        if (starActions.IsTraveling) // Checls of tje star has been thrown, and then starts trail system and stops recall for a neater look
        {
            trailParticles.Play();
            recallParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        else
        {
            trailParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        UpdateStarShimmerSFX(distanceToPlayer);
    }

    void UpdateStarShimmerSFX(float distanceToPlayer)
    {
        PLAYBACK_STATE state;
        starShimmerSFX.getPlaybackState(out state);

        if (distanceToPlayer <= recallRange)
        {
            if (state == PLAYBACK_STATE.STOPPED || state == PLAYBACK_STATE.STOPPING)
            {
                starShimmerSFX.start();
            }
        }
        else
        {
            if (state == PLAYBACK_STATE.PLAYING)
            {
                starShimmerSFX.stop(STOP_MODE.ALLOWFADEOUT);
            }
        }
    }
}
