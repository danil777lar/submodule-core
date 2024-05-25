using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSegment
{
    public readonly Vector3 Min;
    public readonly Vector3 Max;

    public bool Hidden;

    public WallSegment Next;
    public WallSegment Prev;
    public WallSegment Upper;
    public WallSegment Lower;

    public WallSegment(Vector3 min, Vector3 max)
    {
        Min = min;
        Max = max;
    }
}
