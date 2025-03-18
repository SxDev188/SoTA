using UnityEngine;

public class RadialEffectManager : MonoBehaviour
{
    void Update()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Shader.SetGlobalVector("_PlayerPosition", player.transform.position);
        }
    }
}