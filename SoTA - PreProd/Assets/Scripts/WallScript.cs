using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.WSA;

public class WallScript : MonoBehaviour, IActivatable
{
    Vector3 defaultPosition; //position when NOT activated
    Transform activatedTransform; //contains position activated (is fetched from a child object)

    bool isActive = false;

    void Start()
    {
        defaultPosition = transform.position;
        
        if (transform.childCount <= 0)
        {
            Debug.Log("Error. Wall needs child to indicate its activated position!");
        } else
        {
            activatedTransform = transform.GetChild(0).transform;
        }
    }

    public void Activate()
    {
        //Currently a mess but works. Looks this way becase button does not call Deactivate method atm.
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
