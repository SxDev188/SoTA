using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeScript : MonoBehaviour, IActivatable
{
    [SerializeField] bool isActive = true;

    private void Start()
    {
        if (isActive == true)
        {
            gameObject.SetActive(true);
        }
        else if (isActive == false)
        {
            gameObject.SetActive(false);
        }
    }
    public void Activate()
    {
        if(isActive == true)
        {
            gameObject.SetActive(false);
            isActive = false;
        }
        else if (isActive == false)
        {
            gameObject.SetActive(true);
            isActive = true;
        }

    }

    public void Deactivate()
    {
        if (isActive == true)
        {
            gameObject.SetActive(false);
            isActive = false;
        }
        else if (isActive == false)
        {
            gameObject.SetActive(true);
            isActive = true;
        }
    }
}
