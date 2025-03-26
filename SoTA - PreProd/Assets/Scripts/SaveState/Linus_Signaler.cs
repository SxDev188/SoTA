using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Linus_Signaler : MonoBehaviour
{
    [SerializeField] protected List<GameObject> puzzleElements = new List<GameObject>();
    protected bool isPushed = false;
    public bool IsActive { get { return isPushed; } }
    protected EventInstance signalerSFX;
    public void SetElements()
    {
        foreach (GameObject puzzleElement in puzzleElements)
        {
            IActivatable activatable = puzzleElement.GetComponent<IActivatable>();
            if (activatable != null)
            {
                if (isPushed)
                {
                    activatable.Activate();
                }
                else
                {
                    activatable.Deactivate();
                }
            }
        }
    }
}
