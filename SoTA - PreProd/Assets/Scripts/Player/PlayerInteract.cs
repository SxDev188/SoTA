using TMPro;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float interactionRange = 0.5f;

    public float InteractionRange 
    { 
        get
        {
            return interactionRange;
        } 
    }

    private Collider InsideInteractRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange);
        Collider closestCollider = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in hitColliders)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCollider = collider;
                }
            }
        }

        return closestCollider;
    }

    private void OnInteract()
    {
        Collider closestCollider = InsideInteractRange();
        if (closestCollider != null)
        {
            closestCollider.GetComponent<IInteractable>().Interact();
        }
    }

    // Visualizes the interaction range in Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}