using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Linq;
using System.Collections.Generic;

public class LightTracker : MonoBehaviour
{
    private RadialColorRenderFeature feature;
    

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
        if (star == null)
        {
            GameObject starObject = GameObject.FindWithTag("Star");
            if (starObject != null)
                star = starObject.transform;
        }

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

    public void RegisterLightSource(Transform lightSource)
    {
        lightSources.Add(lightSource);
        //Debug.Log($"Registered light source: {lightSource.name}");
        Update();
    }

    public void RefreshLightSources()
    {
        List<Vector4> lightSourcePositions = new List<Vector4>();

        foreach (Transform t in lightSources)
        {
            LampScript lamp = t.GetComponent<LampScript>();
            if (lamp != null && lamp.IsLit)
            {
                Vector4 pos = Camera.main.WorldToViewportPoint(t.position + 1.5f * Vector3.down);
                lightSourcePositions.Add(pos);
            }
        }

        // Ensure exactly MAX_LIGHT_SOURCE_NUM elements (10) so the shader doesn't read junk data
        while (lightSourcePositions.Count < 10)
        {
            lightSourcePositions.Add(new Vector4(-1, -1, 0, 0)); // Placeholder for inactive lights
        }

        feature.SetLightPositions(lightSourcePositions);
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

        //Debug.Log($"LightTracker Update Running. Total registered lights: {lightSources.Count}");

        List<Vector4> lightSourcePositions = new List<Vector4>();
        foreach (Transform t in lightSources)
        {
            //Debug.Log($"Checking light source: {t.name}");

            LampScript lamp = t.GetComponent<LampScript>();
            //if (lamp != null)
            //{
            //    Debug.Log($"Lamp {t.name} found. isLit = {lamp.isLit}");
            //}

            if (lamp != null && lamp.IsLit)
            {
                Vector4 pos = Camera.main.WorldToViewportPoint(t.position);
                lightSourcePositions.Add(pos);
                //Debug.Log($"Light source {t.name} is lit at {pos}");
            }
        }

        feature.SetLightPositions(lightSourcePositions);
    }
}