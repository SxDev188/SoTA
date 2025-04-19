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
        segments.Add(segment);
    }
    public void RemoveSegment(LevelSegment segment)
    {
        bool newSegment = false;
        if(segment == GetCurrentSegment())
            newSegment = true;
        
        segments.Remove(segment);
        
        if(newSegment)
            SegmentChanged?.Invoke();
    }
   

    public LevelSegment GetCurrentSegment()
    {
        return segments[0];
    }
}
