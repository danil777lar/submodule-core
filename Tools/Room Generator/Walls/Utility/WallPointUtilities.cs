using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dreamteck.Splines;
using Larje.Core.Tools.RoomGenerator;
using UnityEngine;

public static class WallPointUtilities
{
    public static List<double> GetHeightPoints(SplineWall wall)
    {
        List<double> heights = new List<double> { 0 };

        float lastHeights = 0f;

        foreach (SplineWallRow wallPart in wall.Config.WallParts)
        {
            float partHeightPercent = wallPart.Weight / wall.Config.GetWeightsSum();
            float partHeight = wall.Config.Height * partHeightPercent;
            lastHeights += partHeight;
            heights.Add(lastHeights);
        }

        AddHoleHeights(wall, heights);

        heights.Sort();

        return heights;
    }

    public static List<double> GetPercentPoint(SplineWall wall)
    {
        List<double> percents = new List<double>();
        List<Vector3> points = GetPoints(wall);
        foreach (Vector3 point in points)
        {
            percents.Add(wall.SplineInstance.Project(point).percent);
        }

        AddHolePercents(wall, percents);

        percents.Sort();

        percents.Add(1);

        return percents;
    }

    public static Vector3 GetDirectionByPercent(SplineWall wall, double percent)
    {
        Dictionary<double, Vector3> directions = GetPointsDirections(wall);

        if (directions.ContainsKey(percent))
        {
            return directions[percent];    
        }

        double percentPrev = directions.Keys.ToList().FindAll(key => key <= percent).Max();
        double percentNext = directions.Keys.ToList().FindAll(key => key >= percent).Min();
        double lerpValue = (percent - percentPrev) / (percentNext - percentPrev);
        
        return Vector3.Lerp(directions[percentPrev], directions[percentNext], (float)lerpValue).normalized;
    }
    
    private static Dictionary<double, Vector3> GetPointsDirections(SplineWall wall)
    {
        Dictionary<double, Vector3> directions = new Dictionary<double, Vector3>();
        int lastIndex = wall.SplineInstance.pointCount - 1;
        for (int i = 0; i < wall.SplineInstance.pointCount; i++)
        {
            double percent = wall.SplineInstance.GetPointPercent(i);
            
            int prevPoint = wall.SplineInstance.isClosed && i == 0 ? lastIndex : Mathf.Max(i - 1, 0);
            int nextPoint = wall.SplineInstance.isClosed && i == lastIndex ? 0 : Mathf.Min(i + 1, lastIndex);
            
            Vector3 prevPointDirection = wall.SplineInstance.GetPointPosition(prevPoint) - wall.SplineInstance.GetPointPosition(i);
            Vector3 nextPointDirection = wall.SplineInstance.GetPointPosition(nextPoint) - wall.SplineInstance.GetPointPosition(i);
            
            Vector3 direction = nextPointDirection.normalized - prevPointDirection.normalized;
            direction = new Vector3(direction.z, direction.y, -direction.x).normalized; 
            directions.Add(percent, direction);
        }

        if (wall.SplineInstance.isClosed)
        {
            directions.Add(1, directions[0]);
        }

        return directions;
    }

    private static void AddHoleHeights(SplineWall wall, List<double> heights)
    {
        foreach (SplineWallHole hole in wall.GetComponentsInChildren<SplineWallHole>())
        {
            SplineWallHole.Data holeData = hole.GetData();

            if (holeData.YFrom < wall.Config.Height && holeData.YFrom > 0)
            {
                heights.Add(holeData.YFrom);
            }

            if (holeData.YTo < wall.Config.Height && holeData.YTo > 0)
            {
                heights.Add(holeData.YTo);
            }
        }
    }

    private static void AddHolePercents(SplineWall wall, List<double> percents)
    {
        foreach (SplineWallHole hole in wall.GetComponentsInChildren<SplineWallHole>())
        {
            SplineWallHole.Data holeData = hole.GetData();
            percents.Add(holeData.XFrom);
            percents.Add(holeData.XTo);
        }
    }

    private static List<Vector3> GetPoints(SplineWall wall)
    {
        List<Vector3> points = new List<Vector3>();
        List<SplinePoint> splinePoints = wall.SplineInstance.GetPoints().ToList();

        if (wall.SplineInstance.isClosed)
        {
            splinePoints.Add(splinePoints[0]);
        }

        AddSplinePoints(wall, points, splinePoints);

        return points;
    }

    private static void AddSplinePoints(SplineWall wall, List<Vector3> points, List<SplinePoint> splinePoints)
    {
        for (int i = 0; i < splinePoints.Count - 1; i++)
        {
            points.Add(splinePoints[i].position);
            float lengthToNext = wall.SplineInstance.CalculateLength(wall.SplineInstance.GetPointPercent(i),
                wall.SplineInstance.GetPointPercent(i + 1));
            int pointsCount = Mathf.RoundToInt(lengthToNext * wall.SegmentsPerUnit);
            for (int j = 1; j < pointsCount; j++)
            {
                float percent = Mathf.Lerp(
                    (float)wall.SplineInstance.GetPointPercent(i),
                    (float)wall.SplineInstance.GetPointPercent(i + 1),
                    (float)j / (float)pointsCount);

                points.Add(wall.SplineInstance.EvaluatePosition(percent));
            }
        }
    }
}