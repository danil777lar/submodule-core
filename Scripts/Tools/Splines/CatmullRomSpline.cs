using UnityEngine;

namespace Larje.Core.Tools.Spline
{
    public class CatmullRomSpline : Spline
    {
        public override Vector3 Evaluate(float t)
        {
            int segmentCount = _points.Length - (Closed ? 0 : 1);
            float scaledT = t * segmentCount;
            int i = Mathf.FloorToInt(scaledT);
            float localT = scaledT - i;

            Vector3 p0 = GetPointAt(i - 1);
            Vector3 p1 = GetPointAt(i);
            Vector3 p2 = GetPointAt(i + 1);
            Vector3 p3 = GetPointAt(i + 2);
            
            Vector3 a4 = p1;
            Vector3 a3 = (p2 - p0) / 2.0f;
            Vector3 a1 = (p3 - p1) / 2.0f - 2.0f*p2 + a3 + 2.0f*a4;
            Vector3 a2 = 3.0f*p2 - (p3 - p1) / 2.0f - 2.0f*a3 - 3.0f*a4;

            return a1 * Mathf.Pow(localT, 3) + a2 * Mathf.Pow(localT, 2) + a3 * localT + a4;
        }
    }
}