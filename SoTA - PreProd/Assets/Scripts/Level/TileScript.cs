using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    Color color;
    ParticleSystemRenderer particleSystemRenderer;
    MeshRenderer meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        if (GetComponentInChildren<ParticleSystemRenderer>() != null)
        {
            particleSystemRenderer = GetComponentInChildren<ParticleSystemRenderer>();
            meshRenderer = GetComponent<MeshRenderer>();
            color = particleSystemRenderer.material.color;
            meshRenderer.materials[4].color = color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
