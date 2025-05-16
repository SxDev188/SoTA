using TMPro;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Author: Sixten
/// Ignore all the stupid comments or names :p
/// </summary>

public class AudioMenu : MonoBehaviour
{
    // We should not keep a bunch of legacy code in the src file since it only bloats. 
    // Also, stop commenting out variables that are not in use anymore. Delete them.
    public void SetMasterVolume(float volume)
    {
        AudioManager.Instance.masterVolume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        AudioManager.Instance.SFXVolume = volume;
    }

    public void SetMusicVolume(float volume)
    {
        AudioManager.Instance.musicVolume = volume;
    }
}
