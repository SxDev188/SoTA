using UnityEngine;

/// <summary>
/// Author: Sixten
/// Ignore all the stupid comments or names :p
/// </summary>

public class ObjectDialougeScript : MonoBehaviour, IInteractable
{
    //IIRC this whole source file is from the tutorial but with minor (if any) changes
    // Written by myself though

    [SerializeField] SO_Dialogue convo;

    public void Interact()
    {
        DialogueManager.Instance.Queue(convo);
    }
}
