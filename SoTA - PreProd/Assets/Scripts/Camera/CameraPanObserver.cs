using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class CameraPanObserver : MonoBehaviour
{
    [SerializeField] private PlayerSegment player;
    [SerializeField] private float panSpeed = 5f;
    private Vector3 cameraStartPosition;
    private void OnSegmentChanged()
    {
        Vector3 segmentPosition = player.GetCurrentSegmentPosition();
        Vector3 targetPosition = cameraStartPosition + segmentPosition;
        StopAllCoroutines();
        StartCoroutine(SmoothPan(targetPosition));
    }
    private void Awake()
    {
        cameraStartPosition = transform.position;
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            player = playerObject.GetComponent<PlayerSegment>();
        }
        if (player != null)
        {
            player.SegmentChanged += OnSegmentChanged;
        }
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.SegmentChanged -= OnSegmentChanged;
        }
    }
    private IEnumerator SmoothPan(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, panSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
    }
}
