using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeScript : MonoBehaviour, IActivatable
{

    [SerializeField] PlayerController playerController;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           playerController.health = 0;
            Debug.Log("Player has died");
        }
    }

    public void Activate()
    {
        transform.position += new Vector3(0, 10, 0);
    }

    public void Deactivate()
    {
        transform.position += new Vector3(0, -10, 0);
    }
}
