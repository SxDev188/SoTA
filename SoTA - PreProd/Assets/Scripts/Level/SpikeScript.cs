using UnityEngine;

public class SpikeScript : MonoBehaviour, IActivatable
{
    [SerializeField] bool startsAsActive = true;

    private void Start()
    {
        if (startsAsActive == true)
        {
            gameObject.SetActive(true);
        }
        else if (startsAsActive == false)
        {
            gameObject.SetActive(false);
        }
    }
    public void Activate()
    {
        if (startsAsActive == true)
        {
            gameObject.SetActive(false);
        }
        else if (startsAsActive == false)
        {
            gameObject.SetActive(true);
        }
    }

    public void Deactivate()
    {
        if (startsAsActive == true)
        {
            gameObject.SetActive(true);
        }
        else if (startsAsActive == false)
        {
            gameObject.SetActive(false);
        }
    }
}
