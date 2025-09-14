using UnityEngine;

public static class GizmosExtensions
{
    public static Vector3[] DrawCircle(float radius, Vector3 position, Vector3 up, int segments = 12)
    {
        Vector3[] points = new Vector3[segments];
        
        Vector3 right = Vector3.Cross(up, Vector3.forward).normalized * radius;
        Vector3 forward = Vector3.Cross(right, up).normalized * radius;

        Vector3 lastPoint = position + right;
        float angleStep = 360f / segments;

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i;
            Quaternion rotation = Quaternion.AngleAxis(angle, up);
            Vector3 nextPoint = position + rotation * right;

            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
            points[i - 1] = nextPoint;
        }

        return points;
    }
    
    public static void DrawCylinder(float radius, float height, Vector3 position, Vector3 up, int segments = 12)
    {
        Vector3[] pointsDown = DrawCircle(radius, position, up, segments);
        Vector3[] pointsUp = DrawCircle(radius, position + up.normalized * height, up, segments);
        
        for (int i = 0; i < segments; i++)
        {
            Gizmos.DrawLine(pointsDown[i], pointsUp[i]);
        }
    }
    
    public static void DrawCylinderHollow(float outerRadius, float innerRadius, float height, Vector3 position, Vector3 up, int segments = 12)
    {
        Vector3[] pointsDownOuter = DrawCircle(outerRadius, position, up, segments);
        Vector3[] pointsUpOuter = DrawCircle(outerRadius, position + up.normalized * height, up, segments);
        
        Vector3[] pointsDownInner = DrawCircle(innerRadius, position, up, segments);
        Vector3[] pointsUpInner = DrawCircle(innerRadius, position + up.normalized * height, up, segments);
        
        for (int i = 0; i < segments; i++)
        {
            Gizmos.DrawLine(pointsDownOuter[i], pointsUpOuter[i]);
            Gizmos.DrawLine(pointsDownInner[i], pointsUpInner[i]);
            
            Gizmos.DrawLine(pointsUpOuter[i], pointsUpInner[i]);
            Gizmos.DrawLine(pointsDownOuter[i], pointsDownInner[i]);
        }
    }
}
