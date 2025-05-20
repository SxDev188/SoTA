using UnityEngine;

/// <summary>
/// Author: Sixten
/// Ignore any the stupid comments or names :p
/// </summary>

public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            // Rotera objektet så det alltid vänder sig mot kameran
            transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward,
                             cam.transform.rotation * Vector3.up);
        }
    }
}
