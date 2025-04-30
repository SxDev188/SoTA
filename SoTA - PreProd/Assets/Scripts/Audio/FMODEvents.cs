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
    
    [field: Header("Low Health Warning SFX")]
    [field: SerializeField] public EventReference LowHealthWarningSFX { get; private set; }
    
    [field: Header("Death SFX")]
    [field: SerializeField] public EventReference DeathSFX { get; private set; }
    
    [field: Header("Background Music")]
    [field: SerializeField] public EventReference BackgroundMusic { get; private set; }
    
    [field: Header("Ambience")]
    [field: SerializeField] public EventReference Ambience { get; private set; }

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
