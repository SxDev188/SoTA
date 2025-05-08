using UnityEngine;

public class SpikeScript : MonoBehaviour, IActivatable
{
    [SerializeField] bool startsAsActive = true;

    private void Start()
    {
        if (startsAsActive == true)
        {
            //gameObject.SetActive(true);
            ResetRotation();
        }
        else if (startsAsActive == false)
        {
            //gameObject.SetActive(false);
            Flip();
        }
    }
    public void Activate()
    {
        if (startsAsActive == true)
        {
            //gameObject.SetActive(false);
            Flip();
        }
        else if (startsAsActive == false)
        {
            //gameObject.SetActive(true);
            ResetRotation();
        }
    }

    public void Deactivate()
    {
        if (startsAsActive == true)
        {
            //gameObject.SetActive(true);
            ResetRotation();
        }
        else if (startsAsActive == false)
        {
            //gameObject.SetActive(false);
            Flip();
        }
    }

    private void Flip()
    {
        // Flip the object by rotating 180 degrees on the X-axis
        transform.Rotate(180f, 0f, 0f);
    }

    private void ResetRotation()
    {
        // Reset the object's rotation to its original state (zero rotation)
        transform.rotation = Quaternion.identity;
    }
}
