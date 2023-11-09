using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Tools.RoomGenerator
{
    [CreateAssetMenu(fileName = "Spline Wall Config", menuName = "Configs/Spline Walls/Wall Config")]
    public class SplineWallConfig : ScriptableObject
    {
        [SerializeField, Min(0f)] private float height;
        [SerializeField, Min(0f)] private float width;
        [SerializeField] private List<WallPart> wallParts = new List<WallPart>();

        public float Height => height;
        public float Width => width;
        public WallPart[] WallParts => wallParts.ToArray();

        public float GetWeightsSum()
        {
            float sum = 0;
            wallParts.ForEach(x => sum += x.Weight);
            return sum;
        }
        
        [Serializable]
        public class WallPart
        {
            [SerializeField, Min(0f)] private float widthMultiplier = 1;
            [SerializeField, Min(0)] private float weight = 1;
            [SerializeField] private Material material;

            public float WidthMultiplier => widthMultiplier;
            public float Weight => weight;
            public Material Material => material;
        }
    }
}