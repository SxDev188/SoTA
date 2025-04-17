using UnityEngine;

public class SpawnPointScript : MonoBehaviour
{
    PlayerController playerController;
    Vector3 spawmPointPosition;
    void Start()
    {
        spawmPointPosition = transform.position + new Vector3(0, 0f, 0);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        Spawn();
        SaveStateManager.Instance.Save();
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
        playerController.SetPlayerPosition(spawmPointPosition);
    }
}
