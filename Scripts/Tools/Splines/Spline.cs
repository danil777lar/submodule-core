using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Tools.Spline
{
    public abstract class Spline
    {
        private Vector3[] _points;

        public float Length { get; private set; }
        public IReadOnlyCollection<Vector3> Points => _points;

        public abstract Vector3 Evaluate(float t);
        
        public Spline SetPoints(Vector3[] points)
        {
            _points = points;
            CalculateLength();

            return this;
        }

        public Vector3 MoveTowards(float fromLength, float distance)
        {
            return Vector3.zero;
        }
        
        private void CalculateLength()
        {
            Length = 0f;
            float step = 0.1f;
            for (float i = 0; i < 1f; i = i + step)
            {
                Vector3 pointA = Evaluate(i);
                Vector3 pointB = Evaluate(i + step);
                Length += Vector3.Distance(pointA, pointB);
            }
        }
    }
}