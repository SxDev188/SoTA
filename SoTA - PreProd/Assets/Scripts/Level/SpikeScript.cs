using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeScript : MonoBehaviour, IActivatable
{
    public void Activate()
    {
        gameObject.SetActive(false);
    }

    public void Deactivate()
    {
        gameObject.SetActive(true);
    }
}
