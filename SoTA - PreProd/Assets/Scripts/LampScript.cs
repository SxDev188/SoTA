using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampScript : MonoBehaviour, IActivatable

{
    private LightTracker tracker;
    public bool IsLit { get; private set; } = false;

    void Start()
    {
        tracker = GameObject.Find("RadialColorManager").GetComponent<LightTracker>();
        tracker.RegisterLightSource(transform);
    }

    public void Activate()
    {
        if (!IsLit)
        {
            IsLit = true;
            tracker.RefreshLightSources();
        }
    }

    public void Deactivate()
    {
        if (IsLit)
        {
            IsLit = false;
            tracker.RefreshLightSources();
        }
    }
}
