using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeScript : MonoBehaviour, IActivatable
{
    public void Activate()
    {
        transform.position += new Vector3(0, 10, 0);
    }

    public void Deactivate()
    {
        transform.position += new Vector3(0, -10, 0);
    }
}
