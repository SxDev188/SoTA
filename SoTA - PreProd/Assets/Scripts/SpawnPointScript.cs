using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointScript : MonoBehaviour
{
   [SerializeField] PlayerController playerController;
   [SerializeField] Transform playerTransform;
    Transform spawnPointTransform;


    void Start()
    {
        spawnPointTransform = gameObject.GetComponent<Transform>();
        Spawn();
    }

    void Update()
    {
        if(playerController.currentHealth <= 0)
        {
            playerController.InteruptMovement();
            playerController.enabled = false;
            Spawn();
            playerController.currentHealth = playerController.maxHealth;
            playerController.enabled = true;
        }

    }
    public void Spawn()
    {
        playerTransform.position = spawnPointTransform.position + new Vector3(0, 0.5f, 0);
    }

}
