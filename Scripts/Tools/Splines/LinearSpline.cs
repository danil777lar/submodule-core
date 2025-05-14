using UnityEngine;

namespace Larje.Core.Tools.Spline
{
    public class LinearSpline : Spline
    {
        public override Vector3 Evaluate(float t)
        {
            if (_points == null || _points.Length < 2)
            {
                return (_points.Length == 1 ? _points[0] : Vector3.zero); 
            }

            t = Mathf.Clamp01(t);
            float totalLength = _points.Length - 1;
            float scaledT = t * totalLength;

            int i = Mathf.FloorToInt(scaledT);
            if (i >= _points.Length - 1)
            {
                i = _points.Length - 2;
            }

            float localT = scaledT - i;

            Vector3 p0 = _points[i];
            Vector3 p1 = _points[i + 1];

            return Vector3.Lerp(p0, p1, localT);
        }

        protected override void CalculateLength()
        {
            Length = 0f;
            for (int i = 0; i < _points.Length - 1; i++)
            {
                Length += Vector3.Distance(_points[i], _points[i + 1]);
            }
        }
        
        protected override Vector3[] GetDrawLineSteps(float step)
        {
            return _points;
        }
    }
}