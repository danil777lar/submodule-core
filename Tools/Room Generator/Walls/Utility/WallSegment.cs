#if DREAMTECK_SPLINES

using MoreMountains.Tools;
using UnityEngine;

namespace Larje.Core.Tools.RoomGenerator
{
    public class WallSegment
    {
        public readonly Vector3 Min;
        public readonly Vector3 Max;

        public bool Hidden;

        public bool IsUpperInRow;
        public bool IsLowerInRow;

        public int RowIndex;

        public double PercentFrom;
        public double PercentTo;

        public float WidthMultiplierTop;
        public float WidthMultiplierBottom;

        public Color VertexColorTop;
        public Color VertexColorBottom;

        public WallSegment Next;
        public WallSegment Prev;
        public WallSegment Upper;
        public WallSegment Lower;

        public Vector3 OffsetFrom;
        public Vector3 OffsetTo;

        public Vector3[] Corners => new[] { Min, Min.MMSetY(Max.y), Max, Max.MMSetY(Min.y) };
        public Vector3 Forward => (Max.MMSetY(0) - Min.MMSetY(0)).normalized;
        public Vector3 Right => new Vector3(Forward.z, Forward.y, -Forward.x);

        public WallSegment(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }
    }
}

#endif