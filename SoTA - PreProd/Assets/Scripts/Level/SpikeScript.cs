using UnityEngine;

public class SpikeScript : MonoBehaviour, IActivatable
{
    [SerializeField] bool startsAsActive = true;

    private void Start()
    {
        if (startsAsActive == true)
        {
            ResetRotation();
        }
        else if (startsAsActive == false)
        {
            Flip();
        }
    }
    public void Activate()
    {
        if (startsAsActive == true)
        {
            Flip();
        }
        else if (startsAsActive == false)
        {
            ResetRotation();
        }
    }

    public void Deactivate()
    {
        if (startsAsActive == true)
        {
            ResetRotation();
        }
        else if (startsAsActive == false)
        {
            Flip();
        }
    }

    private void Flip()
    {
        transform.Rotate(180f, 0f, 0f);
    }

    private void ResetRotation()
    {
        transform.rotation = Quaternion.identity;
    }
}
