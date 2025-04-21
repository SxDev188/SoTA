using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerSegment : MonoBehaviour
{
    List<LevelSegment> segments = new List<LevelSegment>();
    public event Action SegmentChanged;
    public void AddSegment(LevelSegment segment)
    {
        if (!segments.Contains(segment))
            segments.Add(segment);
        if (segments.Count == 1)
            SegmentChanged?.Invoke();
    }
    public void RemoveSegment(LevelSegment segmentToRemove)
    {
        bool newCurrentSegment = false;
        if(segments.Count != 1 && segmentToRemove == GetCurrentSegment())
            newCurrentSegment = true;
        
        segments.Remove(segmentToRemove);
        
        if(newCurrentSegment)
            SegmentChanged?.Invoke();
    }
    private LevelSegment GetCurrentSegment()
    {
        return segments[0];
    }
    public void ClearSegments()
    {
        segments.Clear();
    }
    public Vector3 GetCurrentSegmentPosition()
    {
        return GetCurrentSegment().GetSegmentPosition();
    }
}
