using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.WSA;

public class WallScript : MonoBehaviour, IActivatable
{
    // Start is called before the first frame update
    Vector3 defaultPosition;
    [SerializeField] Transform activatedTransform;

    bool isActive = false;

    void Start()
    {
        defaultPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Activate()
    {

        if (!isActive)
        {
            transform.position = activatedTransform.position;
            isActive = true;
        }
        else
        {
            Deactivate();
        }

    }

    public void Deactivate()
    {
        //is not called by button atm, idk why
        transform.position = defaultPosition;
        isActive = false;
    }
}
