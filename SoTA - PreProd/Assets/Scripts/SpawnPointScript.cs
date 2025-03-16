using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointScript : MonoBehaviour
{
   [SerializeField] PlayerController playerController;
   [SerializeField] Transform playerTransform;
    Transform spawnPointTransform;
    bool hasRespawned = false;
    void Start()
    {
        Spawn();
    }

    void Update()
    {
        RegainHealth();
        HandleDeath();

    }
    public void Spawn()
    {
        spawnPointTransform = gameObject.GetComponent<Transform>();
        playerTransform.position = spawnPointTransform.position + new Vector3(0, 0.5f, 0);
    }

    void HandleDeath()
    {
        if (playerController.health <= 0)
        {

            Spawn();
            Debug.Log("Player has respawned");
            hasRespawned = true;
        }
    }
    void RegainHealth()
    {
        if (hasRespawned == true)
        {
            playerController.health = 10;
            hasRespawned = false;
        }
    }
}
