using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EndPointScript : MonoBehaviour
{
    [SerializeField] Transform nextSpawnPoint;
    [SerializeField] Vector2 cameraPanDirection;
    [SerializeField] bool isExit;
    PlayerController playerController;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();

        if (playerController == null)
        {
            Debug.LogWarning("PlayerController not found in the scene!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isExit)
            {
                Debug.LogWarning("Exiting game!");
                #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
                #endif
            }

            if (nextSpawnPoint != null)
            {
                playerController.transform.position = nextSpawnPoint.position;
                CameraPanScript.Instance.PanCamera(cameraPanDirection);
            }
            else
            {
                Debug.LogWarning("Next spawn point not assigned!");
            }
        }
    }
}
