using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampScript : MonoBehaviour, IActivatable

{
    private LightTracker tracker;
    public bool isLit = false;

    void Start()
    {
        tracker = GameObject.Find("RadialColorManager").GetComponent<LightTracker>();
        tracker.RegisterLightSource(transform);
    }

    public void Activate()
    {
        if (!isLit)
        {
            isLit = true;
            tracker.RefreshLightSources();
        }
    }

    public void Deactivate()
    {
        if (isLit)
        {
            isLit = false;
            tracker.RefreshLightSources();
        }
    }
}
