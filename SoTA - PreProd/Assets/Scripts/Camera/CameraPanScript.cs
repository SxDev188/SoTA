using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPanScript : MonoBehaviour
{
    public static CameraPanScript Instance;

    public float panSpeed = 5f;
    private Vector3 targetPosition;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        targetPosition = transform.position;
    }

    public void PanCamera(Vector2 direction)
    {
        targetPosition += new Vector3(direction.x * 11f, 0, direction.y * 11f);

        StopAllCoroutines(); 
        StartCoroutine(SmoothPan());
    }

    private IEnumerator SmoothPan()
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, panSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition; 
    }
}
