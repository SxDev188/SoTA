using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Button SFX")]
    [field: SerializeField] public EventReference ButtonSFX { get; private set; }

    [field: Header("Timer Ticking SFX")]
    [field: SerializeField] public EventReference TimerTickingSFX { get; private set; }

    [field: Header("Pressure Plate SFX")]
    [field: SerializeField] public EventReference PressurePlateSFX { get; private set; }
    
    [field: Header("Slither SFX")]
    [field: SerializeField] public EventReference SlitherSound { get; private set; }
    
    [field: Header("Boulder SFX")]
    [field: SerializeField] public EventReference BoulderSFX { get; private set; }

    public static FMODEvents Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("Found more than one FMOD Events instance in the scene.");
        }

        Instance = this;
    }
}
