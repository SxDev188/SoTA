using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Linus_PressurePlateScript : Linus_Signaler
{
    // Start is called before the first frame update
    private List<GameObject> objectsOnPlate = new List<GameObject>();
    void Start()
    {
        signalerSFX = AudioManager.Instance.CreateInstance(FMODEvents.Instance.ButtonSFX);
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Star") || other.CompareTag("Boulder"))
        {
            objectsOnPlate.Add(other.gameObject);
        }

        if (objectsOnPlate.Count > 0)
        {
            if (!isPushed)
            {
                isPushed = true;
                signalerSFX.setParameterByNameWithLabel("ButtonPushState", "PushDown");
                signalerSFX.start();

                SetElements();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Star") || other.CompareTag("Boulder"))
        {
            objectsOnPlate.Remove(other.gameObject);
        }

        if (objectsOnPlate.Count <= 0)
        {
            isPushed = false;
            signalerSFX.setParameterByNameWithLabel("ButtonPushState", "PushUp");
            signalerSFX.start();


            SetElements();
        }
    }
}
