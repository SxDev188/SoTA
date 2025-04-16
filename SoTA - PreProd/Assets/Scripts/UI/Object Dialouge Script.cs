using UnityEngine;

public class ObjectDialougeScript : MonoBehaviour, IInteractable
{

    [SerializeField] SO_Dialogue convo;

    public void Interact()
    {
        DialogueManager.Instance.Queue(convo);
    }
}
