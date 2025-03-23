using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Linq;
using System.Collections.Generic;

public class LightTracker : MonoBehaviour
{
    [SerializeField]
    private Transform star;
    [SerializeField]
    private List<Transform> lightSources;
    [SerializeField]
    private ScriptableRendererData rendererData;
    [SerializeField]
    private float smoothSpeed = 20f;

    private RadialColorRenderFeature feature;
    private Vector4 smoothedStarPosition;


    void Start()
    {
        if (rendererData == null || star == null)
        {
            Debug.LogError("LightTracker: Missing star or renderer data!");
            return;
        }

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

        // Checks Star position and sets color around it
        Vector3 starViewportPos = Camera.main.WorldToViewportPoint(star.position + 0.25f * Vector3.down);
        Vector4 targetStarPosition = new Vector4(starViewportPos.x, starViewportPos.y, 0, 0);
        // smoothedStarPosition is currently needed to prevent "shaking" when Star is held by player
        smoothedStarPosition = Vector4.Lerp(smoothedStarPosition, targetStarPosition, Time.deltaTime * smoothSpeed);
        feature.SetStarPosition(smoothedStarPosition);

        // Checks additional lightsources and sets color around them
        List<Vector4> lightSourcePositions = new List<Vector4>();
        foreach (Transform t in lightSources)
        {
            lightSourcePositions.Add(Camera.main.WorldToViewportPoint(t.position));
        }
        feature.SetLightPositions(lightSourcePositions);
    }
}