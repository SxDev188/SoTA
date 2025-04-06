using UnityEngine;

public class SpikeScript : MonoBehaviour, IActivatable
{
    public void Activate()
    {
        gameObject.SetActive(false);
    }

    public void Deactivate()
    {
        gameObject.SetActive(true);
    }
}
