using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Linq;
using System.Collections.Generic;

public class StarTracker : MonoBehaviour
{
    public Transform star;
    public ScriptableRendererData rendererData; // Assign URP Renderer Data asset
    public float smoothSpeed = 20f;

    private RadialColorRenderFeature feature;
    private Vector4 smoothedStarPosition;

    [SerializeField]
    private List<Transform> lightSources;

    void Start()
    {
        if (rendererData == null || star == null)
        {
            Debug.LogError("StarTracker: Missing star or renderer data!");
            return;
        }

        // Find the RadialColorRenderFeature inside the Renderer Data
        feature = rendererData.rendererFeatures
            .OfType<RadialColorRenderFeature>()
            .FirstOrDefault();

        if (feature != null)
        {
            Debug.Log("Star assigned to RadialColorRenderFeature.");
        }
        else
        {
            Debug.LogError("RadialColorRenderFeature not found in Renderer Data!");
        }
    }

    void Update()
    {
        if (star == null || feature == null) return;

        // Convert star position to normalized viewport coordinates (0 to 1)
        Vector3 starViewportPos = Camera.main.WorldToViewportPoint(star.position + 0.25f * Vector3.down);

        // Make sure we're sending a normalized position (x, y) as float4 (normalize z and w to 0)
        Vector4 targetStarPosition = new Vector4(starViewportPos.x, starViewportPos.y, 0, 0);

        // Smoothly interpolate between current and target positions
        smoothedStarPosition = Vector4.Lerp(smoothedStarPosition, targetStarPosition, Time.deltaTime * smoothSpeed);

        // Send the smoothed star position to the shader via the render feature
        feature.SetStarPosition(smoothedStarPosition);

        List<Vector4> lightSourcePositions = new List<Vector4>();
        foreach (Transform t in lightSources)
        {
            lightSourcePositions.Add(Camera.main.WorldToViewportPoint(t.position));
        }
        feature.SetLightPositions(lightSourcePositions);
    }
}