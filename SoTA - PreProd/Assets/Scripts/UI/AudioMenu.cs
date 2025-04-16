using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMenu : MonoBehaviour
{

    // Taken from learn unity/peer review, needs modification to work with FMOD

    [SerializeField] AudioMixer audioMixer;

    [SerializeField] TMP_Text masterText;
    [SerializeField] TMP_Text sfxText;
    [SerializeField] TMP_Text ambienceText;

    public void SetMasterVolume(float volume)
    {
        int finalVol = Mathf.Clamp((int)volume * 10, 0, 20);
        masterText.text = finalVol.ToString();
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        int finalVol = Mathf.Clamp((int)volume * 10, 0, 20);
        sfxText.text = finalVol.ToString();
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }

    public void SetAmbienceVolume(float volume)
    {
        int finalVol = Mathf.Clamp((int)volume * 10, 0, 20);
        ambienceText.text = finalVol.ToString();
        audioMixer.SetFloat("AmbienceVolume", Mathf.Log10(volume) * 20);
    }
}
