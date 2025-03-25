using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Button SFX")]
    [field: SerializeField] public EventReference ButtonPushedDownSound { get; private set; }
    [field: SerializeField] public EventReference ButtonPushedUpSound { get; private set; }
    [field: SerializeField] public EventReference ButtonPushedFailSound { get; private set; }

    [field: Header("Slither SFX")]
    [field: SerializeField] public EventReference SlitherSound { get; private set; }

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
