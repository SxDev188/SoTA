using UnityEngine;

public class RadialColorController : MonoBehaviour
{
    public Transform player; // Assign in Inspector

    void Update()
    {
        if (player != null)
        {
            Shader.SetGlobalVector("_PlayerPosition", player.position);
        }
    }
}