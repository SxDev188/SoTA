using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class CubeTestScript : MonoBehaviour, IActivatable 
{
    private void Start()
    {
        
    }

    public void Activate()
    {
        transform.position += transform.forward * 5f;
    }

    public void Deactivate()
    {
        transform.position -= transform.forward * 5f;
    }
}
