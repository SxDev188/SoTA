using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateScript : MonoBehaviour
{
    [SerializeField] private List<GameObject> puzzleElements = new List<GameObject>();
    [SerializeField] private float sinkAmount = 0.02f;

    private List<GameObject> objectsOnPlate = new List<GameObject>();
    private bool isPushedDown = false;
    private Vector3 originalPosition;

    private ParticleSystem pressurePlateParticles;

    private EventInstance pressurePlateSFX;


    private void Start()
    {
        originalPosition = transform.position;
        pressurePlateParticles = GetComponentInChildren<ParticleSystem>();
        FindParticleColor();
        pressurePlateSFX = AudioManager.Instance.CreateInstance(FMODEvents.Instance.PressurePlateSFX);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boulder") && !other.gameObject.GetComponent<BoulderPlayerPushScript>().IsCurrentlyMoving && !other.gameObject.GetComponent<BoulderStarPushScript>().IsCurrentlyMoving)
        {
            return;
        }

        if (other.CompareTag("Player") || other.CompareTag("Star") || other.CompareTag("Boulder"))
        {
            objectsOnPlate.Add(other.gameObject);
        }

        if (objectsOnPlate.Count > 0)
        {
            //if (other.CompareTag("Boulder") && !other.gameObject.GetComponent<BoulderController>().IsAboutToSnapToFloor)
            //{
            //    return;
            //}

            if (!isPushedDown)
            {
                isPushedDown = true;
                transform.position = originalPosition + Vector3.down * sinkAmount;

                pressurePlateSFX.setParameterByNameWithLabel("PressurePlateState", "PushDown");
                pressurePlateSFX.start();
                Debug.Log("TRIGGER ENTER RUNNING INTERACT() ---> " + other.gameObject.tag);
                Interact();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Boulder") && !other.gameObject.GetComponent<BoulderPlayerPushScript>().IsCurrentlyMoving && !other.gameObject.GetComponent<BoulderStarPushScript>().IsCurrentlyMoving)
        {
            return;
        }

        if (other.CompareTag("Player") || other.CompareTag("Star") || other.CompareTag("Boulder"))
        {
            objectsOnPlate.Remove(other.gameObject);
        } else if (other.CompareTag("Untagged"))
        {
            return;
        }

        if (objectsOnPlate.Count <= 0)
        {
            isPushedDown = false;
            transform.position = originalPosition;

            pressurePlateSFX.setParameterByNameWithLabel("PressurePlateState", "PushUp");
            pressurePlateSFX.start();

            Debug.Log("TRIGGER EXIT RUNNING INTERACT() ---> " + other.gameObject.tag);


            Interact();
        }
    }

    public void Interact()
    {
        foreach (GameObject puzzleElement in puzzleElements)
        {
            IActivatable activatable = puzzleElement.GetComponent<IActivatable>();
            if (activatable != null)
            {
                if (isPushedDown)
                {
                    StartCoroutine(PlayAndStopParticleBurst());
                    activatable.Activate();
                }
                else
                {
                    StartCoroutine(PlayAndStopParticleBurst());
                    activatable.Deactivate();
                }
            }
        }
    }

    IEnumerator PlayAndStopParticleBurst()
    {
        pressurePlateParticles.Play();
        yield return new WaitForSeconds(0.1f); // wait for particles to spawn
        pressurePlateParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private void FindParticleColor()
    {
        Transform pressurePlate = transform.Find("PressurePlate");
        Transform pressurePlateParticlesTransform = transform.Find("PressurePlateParticles");

        if (pressurePlate != null && pressurePlateParticlesTransform != null)
        {
            MeshRenderer sourceRenderer = pressurePlate.GetComponent<MeshRenderer>();
            ParticleSystemRenderer psRenderer = pressurePlateParticlesTransform.GetComponent<ParticleSystemRenderer>();

            if (sourceRenderer != null && psRenderer != null)
            {
                psRenderer.material = sourceRenderer.sharedMaterial;
            }
            else
            {
                Debug.LogWarning("MeshRenderer or ParticleSystemRenderer not found.");
            }
        }
        else
        {
            Debug.LogError("pressurePlate or PressurePlateParticles not found in the hierarchy.");
        }
    }
}
