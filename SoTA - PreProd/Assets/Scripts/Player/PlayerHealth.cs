using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float timeWithoutStar = 10.0f;
    [SerializeField] private float timeToResetLife = 3.0f;
    private float currentHealth;
    private float startingHealth = 1.0f;
    private StarActions starActions;
    private EventInstance deathSFX;
    private EventInstance lowHealthWarningSFX;


    public float CurrentHealth { get { return currentHealth; } }

    // Start is called before the first frame update
    private void Start()
    {
        GameObject star = GameObject.FindGameObjectWithTag("Star");
        starActions = star.GetComponent<StarActions>();
        currentHealth = startingHealth;
        deathSFX = AudioManager.Instance.CreateInstance(FMODEvents.Instance.DeathSFX);
        lowHealthWarningSFX = AudioManager.Instance.CreateInstance(FMODEvents.Instance.LowHealthWarningSFX);

    }

    // Update is called once per frame
    private void Update()
    {
        ManagePlayerHealth();

        PlayLowHealthWarningSound();

        //Sets global PlayerHealth parameter in FMOD --> controls effects on the master audio channel
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("PlayerHealth", currentHealth);
    }
    private void ManagePlayerHealth()
    {
        if (starActions.IsOnPlayer)
        {
            if(currentHealth < startingHealth)
            {
                RestoreHealth();
            }
        }
        else
        {
            DrainHealth();
        }
    }

    private void DrainHealth()
    {
        currentHealth -= (Time.deltaTime / timeWithoutStar);
        if (currentHealth < 0.0f)
        {
            Death();
        }
    }

    private void RestoreHealth()
    {
        currentHealth += (Time.deltaTime / timeToResetLife);
        if (currentHealth > startingHealth)
            currentHealth = startingHealth;
    }

    public void Death()
    {
        SaveStateManager.Instance.Load();
        currentHealth = startingHealth;
        deathSFX.start();
    }

    void PlayLowHealthWarningSound()
    {
        if (currentHealth < 0.47f && currentHealth > 0 && !starActions.IsOnPlayer)
        {
            PLAYBACK_STATE playbackState;
            lowHealthWarningSFX.getPlaybackState(out playbackState);

            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                lowHealthWarningSFX.start();
            }
        }
        else
        {
            PLAYBACK_STATE playbackState;
            lowHealthWarningSFX.getPlaybackState(out playbackState);

            if (playbackState.Equals(PLAYBACK_STATE.PLAYING))
            {
                lowHealthWarningSFX.stop(STOP_MODE.ALLOWFADEOUT);
            }
        }
    }
}
