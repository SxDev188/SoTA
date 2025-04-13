using UnityEngine;

public class SpawnPointScript : MonoBehaviour
{
    Transform playerTransform;
    Transform spawnPointTransform;
    void Start()
    {
        spawnPointTransform = gameObject.GetComponent<Transform>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.GetComponent<Transform>();
        Spawn();
    }

    //void Update()
    //{
    //    if(playerController.currentHealth <= 0)
    //    {
    //        playerController.InteruptMovement();
    //        playerController.enabled = false;
    //        SaveStateManager.Instance.Load();
    //        //Added Load here and removed the call to spawn, by Linus
    //        //Maybe change the check to be were the health gets changed so 
    //        //it won't need to be checked every update
    //        playerController.currentHealth = playerController.maxHealth;
    //        playerController.enabled = true;
    //    }

    //}
    public void Spawn()
    {
        playerTransform.position = spawnPointTransform.position + new Vector3(0, 0.5f, 0);
    }

}
