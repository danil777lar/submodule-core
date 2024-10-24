#if DREAMTECK_SPLINES

using System;
using UnityEngine;

namespace Larje.Core.Tools.RoomGenerator
{
    [Serializable]
    public class SplineWallRow
    {
        [Header("Width")] [SerializeField, Min(0f)]
        private float topWidthMultiplier = 1;

        [SerializeField, Min(0f)] private float bottomWidthMultiplier = 1;
        [Header("Sides")] [SerializeField] private bool drawTop = true;
        [SerializeField] private bool drawBottom = true;

        [Header("Other")] [SerializeField, Min(0)]
        private float weight = 1;

        [SerializeField] private Material material;

        [Header("Vertex Colors")] [SerializeField]
        private Color vertexColorTop;

        [SerializeField] private Color vertexColorBottom;

        public float TopWidthMultiplier => topWidthMultiplier;
        public float BottomWidthMultiplier => bottomWidthMultiplier;
        public bool DrawTop => drawTop;
        public bool DrawBottom => drawBottom;
        public float Weight => weight;
        public Material Material => material;
        public Color VertexColorTop => vertexColorTop;
        public Color VertexColorBottom => vertexColorBottom;
    }
}

#endif