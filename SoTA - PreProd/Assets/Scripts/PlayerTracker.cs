using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Linq;

public class PlayerTracker : MonoBehaviour
{
    public Transform player; // Assign in Inspector
    public ScriptableRendererData rendererData; // Assign URP Renderer Data asset

    private RadialColorRenderFeature feature;

    void Start()
    {
        if (rendererData == null || player == null)
        {
            Debug.LogError("PlayerTracker: Missing player or renderer data!");
            return;
        }

        // Find the RadialColorRenderFeature inside the Renderer Data
        feature = rendererData.rendererFeatures
            .OfType<RadialColorRenderFeature>()
            .FirstOrDefault();

        if (feature != null)
        {
            Debug.Log("Player assigned to RadialColorRenderFeature.");
        }
        else
        {
            Debug.LogError("RadialColorRenderFeature not found in Renderer Data!");
        }
    }

    void Update()
    {
        if (player == null || feature == null) return;

        // Convert player position to normalized viewport coordinates (0 to 1)
        Vector3 playerViewportPos = Camera.main.WorldToViewportPoint(player.position + 0.25f * Vector3.down);

        // Make sure we're sending a normalized position (x, y) as float4 (normalize z and w to 0)
        Vector4 playerViewportPosNormalized = new Vector4(playerViewportPos.x, playerViewportPos.y, 0, 0);

        // Send the normalized player position to the shader via the render feature
        feature.SetPlayerPosition(playerViewportPosNormalized);
    }
}