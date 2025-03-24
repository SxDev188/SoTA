using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampScript : MonoBehaviour, IActivatable

{
    private LightTracker tracker;
    public bool isLit = true;

    void Start()
    {
        tracker = GameObject.Find("RadialColorManager").GetComponent<LightTracker>();
        tracker.RegisterLightSource(transform);
    }

    public void Activate()
    {
        if (!isLit)
        {
            //tracker.TurnOnLightSource(transform);
            isLit = true;
        }
    }

    public void Deactivate()
    {
        if (isLit)
        {
            //tracker.TurnOffLightSource(transform);
            isLit = false;
        }
    }
}
