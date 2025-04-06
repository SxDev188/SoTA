using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float interactionRange = 0.5f;

    private void OnInteract()
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