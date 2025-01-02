using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;

public static class NavMeshPathExtensions
{
    public static float GetLength(this NavMeshPath path)
    {
        float length = 0f;
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            length += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }

        return length;
    }

    public static List<Vector3> GetShiftedCorners(this NavMeshPath path, float distance)
    {
        if (path.corners.Length < 3)
        {
            return path.corners.ToList();
        }
        
        List<Vector3> shiftedCorners = new List<Vector3>();
        shiftedCorners.Add(path.corners[0]);

        for (int i = 1; i < path.corners.Length - 1; i++)
        {
            Vector3 normal = path.corners[i + 1] - path.corners[i - 1];
            Vector3 point = path.corners[i] - path.corners[i - 1];
            Vector3 projection = Vector3.Project(point, normal) + path.corners[i - 1];
            Vector3 direction = (path.corners[i] - projection).normalized;

            if (direction != Vector3.zero)
            {
                float maxDistance = distance;
                Vector3 targetPosition = path.corners[i] + direction * (distance * 2f);
                if (NavMesh.Raycast(path.corners[i], targetPosition, out NavMeshHit hit, NavMesh.AllAreas))
                {
                    maxDistance = hit.distance * 0.5f;
                }

                shiftedCorners.Add(path.corners[i] + direction * Mathf.Min(distance, maxDistance));
            }
        }
        
        shiftedCorners.Add(path.corners[^1]);
        return shiftedCorners;
    }
    
    public static void DrawDebugPath(this NavMeshPath path, Color color)
    {
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            Debug.DrawLine(path.corners[i], path.corners[i + 1], color);
        }
    }
    
    public static bool IsAvailable(this NavMeshPath path, Vector3 targetPosition)
    {
        return path.corners.Length > 0 && path.corners[^1] == targetPosition;
    }
}
