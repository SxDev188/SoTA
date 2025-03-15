using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A Singelton since it is a Manager
public class SaveStateManager : MonoBehaviour
{
    public static SaveStateManager Instance { get; private set; }
    List<IActivatable> activatables = new List<IActivatable>();

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SaveAction()
    {

    }

    public void Save()
    {

    }
    public void Load()
    {

    }
}
