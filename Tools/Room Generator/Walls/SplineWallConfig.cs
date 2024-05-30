#if DREAMTECK_SPLINES

using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Tools.RoomGenerator
{
    [CreateAssetMenu(fileName = "Spline Wall Config", menuName = "Configs/Spline Walls/Wall Config")]
    public class SplineWallConfig : ScriptableObject
    {
        [SerializeField, Min(0f)] private float height;
        [SerializeField, Min(0f)] private float width;
        [SerializeField] private List<SplineWallRow> wallParts = new List<SplineWallRow>();

        public float Height => height;
        public float Width => width;

        public SplineWallRow[] WallParts => wallParts.ToArray();

        public float GetWeightsSum()
        {
            float sum = 0;
            wallParts.ForEach(x => sum += x.Weight);
            return sum;
        }
        
        public void GetRowHeightBounds(SplineWallRow row, out double min, out double max)
        {
            min = 0;
            max = 0;
            foreach (SplineWallRow wallPart in wallParts)
            {
                double previousHeights = (wallPart.Weight / GetWeightsSum()) * height;
                
                if (wallPart == row)
                {
                    max = min + previousHeights;
                    return;
                }
                min += previousHeights;
            }
        }
    }
}

#endif