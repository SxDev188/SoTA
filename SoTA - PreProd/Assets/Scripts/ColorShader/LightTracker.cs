using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Linq;
using System.Collections.Generic;

public class LightTracker : MonoBehaviour
{
    private RadialColorRenderFeature feature;
    
    [SerializeField]
    private Transform star;
    private List<Transform> lightSources = new List<Transform>();
    [SerializeField]
    private ScriptableRendererData rendererData;
    [SerializeField]
    private float smoothSpeed = 20f;
    [SerializeField]
    private float effectRadius = 150f;
    [SerializeField]
    private float effectRadiusSmoothing = 10f;
    [SerializeField]
    private float effectToggle = 1f;

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

    public void TurnOffLightSource(Transform lightSource)
    {
        lightSources.Remove(lightSource);
    }

    public void TurnOnLightSource(Transform lightSource)
    {
        lightSources.Add(lightSource);
    }

    void Update()
    {
        if (star == null || feature == null) return;

        // Checks Star position and sends its position further
        Vector3 starViewportPos = Camera.main.WorldToViewportPoint(star.position + 0.25f * Vector3.down);
        // 0.25f * Vector3.down is position adjustment for the radius center
        Vector4 targetStarPosition = new Vector4(starViewportPos.x, starViewportPos.y, 0, 0);
        // smoothedStarPosition is currently needed to prevent "shaking" when Star is held by player
        smoothedStarPosition = Vector4.Lerp(smoothedStarPosition, targetStarPosition, Time.deltaTime * smoothSpeed);
        feature.SetStarPosition(smoothedStarPosition);

        feature.SetLightEffectRadius(effectRadius);
        feature.SetLightEffectRadiusSmoothing(effectRadiusSmoothing);
        feature.SetEffectToggle(effectToggle);

        // Checks additional lightsources and sends their position further
        List<Vector4> lightSourcePositions = new List<Vector4>();
        foreach (Transform t in lightSources)
        {
            lightSourcePositions.Add(Camera.main.WorldToViewportPoint(t.position + 1.5f * Vector3.down)); 
            // 1.5f * Vector3.down is position adjustment for the radius center
        }

        feature.SetLightPositions(lightSourcePositions);
    }
}