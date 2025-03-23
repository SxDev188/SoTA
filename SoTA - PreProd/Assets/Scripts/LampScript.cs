using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampScript : MonoBehaviour, IActivatable

{
    private LightTracker tracker;
    bool isLit = false;

    // Start is called before the first frame update
    void Start()
    {
        tracker = GameObject.Find("RadialColorManager").GetComponent<LightTracker>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {
        if (!isLit)
        {
            tracker.TurnOnLightSource(transform);
            isLit = true;
        }
    }

    public void Deactivate()
    {
        if (isLit)
        {
            tracker.TurnOffLightSource(transform);
            isLit = false;
        }
    }
}
