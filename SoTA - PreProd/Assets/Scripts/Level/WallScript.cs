using UnityEngine;

public class WallScript : MonoBehaviour, IActivatable
{
    Vector3 defaultPosition;    //position when NOT activated
    Vector3 activatedPosition;  //position activated (is fetched from a child object)

    bool isActive = false;

    void Start()
    {
        defaultPosition = transform.position;

        Transform activatedTransform = transform.Find("ActivatedPosition");

        if (transform.childCount <= 0)
        {
            Debug.Log("Error. Wall needs child to indicate its activated position!");
        } else
        {
            activatedPosition = activatedTransform.position;
        }
    }

    public void Activate()
    {
        if (!isActive)
        {
            transform.position = activatedPosition;
            isActive = true;
        }
    }

    public void Deactivate()
    {
        if(isActive)
        {
            transform.position = defaultPosition;
            isActive = false;
        }
    }
}
