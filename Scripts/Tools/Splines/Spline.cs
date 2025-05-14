using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Tools.Spline
{
    public abstract class Spline
    {
        protected Vector3[] _points;

        public bool Closed { get; protected set; }
        public float Length { get; protected set; }
        public IReadOnlyCollection<Vector3> Points => _points;

        public abstract Vector3 Evaluate(float t);

        public float DistanceToPercent()
        {
            return 0f;
        }
        
        public Spline SetPoints(Vector3[] points)
        {
            _points = points;
            CalculateLength();

            return this;
        }
        
        public Spline SetClosed(bool closed)
        {
            Closed = closed;
            return this;
        }

        public void DrawLineGizmo(Color color, float step = 0.1f)
        {
            Gizmos.color = color;
            Vector3[] steps = GetDrawLineSteps(step);
            for (int i = 0; i < steps.Length - 1; i++)
            {
                Gizmos.DrawLine(steps[i], steps[i + 1]);
            }

            foreach (Vector3 p in _points)
            {
                Gizmos.DrawWireSphere(p, 0.1f);
            }
        }

        public void DrawLineDebug(Color color, float step = 0.1f)
        {
            Vector3[] steps = GetDrawLineSteps(step);
            for (int i = 0; i < steps.Length - 1; i++)
            {
                Debug.DrawLine(steps[i], steps[i + 1], color);
            }
        }
        
        protected virtual void CalculateLength()
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

        protected virtual Vector3[] GetDrawLineSteps(float step)
        {
            List<Vector3> steps = new List<Vector3>();
            for (float i = 0; i < 1f; i = i + step)
            {
                steps.Add(Evaluate(i));
            }
            steps.Add(Evaluate(1f));
            return steps.ToArray();
        }
        
        protected Vector3 GetPointAt(int index)
        {
            if (Closed)
            {
                index = (index + _points.Length) % _points.Length;
            }
            else
            {
                index = Mathf.Clamp(index, 0, _points.Length - 1);
            }

            return _points[index];
        }
    }
}