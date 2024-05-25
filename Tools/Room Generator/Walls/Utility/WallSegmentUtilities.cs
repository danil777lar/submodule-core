using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dreamteck.Splines;
using Larje.Core.Tools.RoomGenerator;
using MoreMountains.Tools;
using UnityEngine;

public static class WallSegmentUtilities
{
    public static List<WallSegment> GetSegments(SplineWall wall)
    {
        List<WallSegment> segments = CreateSegments(wall);
        
        MarkHiddenSegments(wall, segments);
        FillSegmentNeighbours(segments);

        return segments;
    }
    
    private static List<WallSegment> CreateSegments(SplineWall wall)
    {
        List<WallSegment> segments = new List<WallSegment>();
        List<double> heights = WallPointUtilities.GetHeightPoints(wall);
        List<double> percents = WallPointUtilities.GetPercentPoint(wall);

        for (int h = 0; h < heights.Count - 1; h++)
        {
            for (int p = 0; p < percents.Count - 1; p++)
            {
                Vector3 min = wall.SplineInstance.EvaluatePosition(percents[p]) + Vector3.up * (float)heights[h];
                Vector3 max = wall.SplineInstance.EvaluatePosition(percents[p + 1]) + Vector3.up * (float)heights[h + 1];
                segments.Add(new WallSegment(min, max));
            }
        }

        return segments;
    }

    private static void MarkHiddenSegments(SplineWall wall, IReadOnlyCollection<WallSegment> segments)
    {
        foreach (WallInterval interval in GetHideIntervals(wall))
        {
            foreach (WallSegment segment in segments)
            {
                double centerPercent = GetSegmentCenterPercent(wall, segment);
                double centerHeight = (segment.Min.y + segment.Max.y) * 0.5f;
                segment.Hidden |= interval.Contains(centerPercent, centerHeight);
            }
        }
    }

    private static void FillSegmentNeighbours(List<WallSegment> segments)
    {
        foreach (WallSegment segment in segments)
        {
            var segment1 = segment;
            segment.Next = segments.Find(x => x.Min == segment1.Max.MMSetY(segment1.Min.y));
            segment.Prev = segments.Find(x => x.Max == segment.Min.MMSetY(segment.Max.y));

            segment.Upper = segments.Find(x => x.Min == segment.Min.MMSetY(segment.Max.y));
            segment.Lower = segments.Find(x => x.Max == segment.Max.MMSetY(segment.Min.y));
        }
    }
    
    private static double GetSegmentCenterPercent(SplineWall wall, WallSegment segment)
    {
        float distance = Vector3.Distance(segment.Min.XZ(), segment.Max.XZ());
        double fromPercent = wall.SplineInstance.Project(segment.Min).percent;
        return wall.SplineInstance.Travel(fromPercent, distance * 0.5f);
    }
    
    private static List<WallInterval> GetHideIntervals(SplineWall wall)
    {
        List<WallInterval> intervals = new List<WallInterval>();

        wall.GetComponentsInChildren<SplineWallHole>().ToList().ForEach(x =>
        {
            SplineWallHole.Data data = x.GetData();
            intervals.Add(new WallInterval(data.XFrom, data.XTo, data.YFrom, data.YTo));
        });
            
        for (int i = 0; i < wall.HideWallParts.Count; i++)
        {
            if (wall.HideWallParts.ToArray()[i])
            {
                double from = wall.SplineInstance.GetPointPercent(i);
                double to = i < wall.HideWallParts.Count - 1 ? wall.SplineInstance.GetPointPercent(i + 1) : 1;
                intervals.Add(new WallInterval(from, to, 0f, wall.Config.Height));
            }
        }

        return intervals;
    } 
}
