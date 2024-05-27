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
        FillOffsets(wall, segments);
        FillWidthMultipliers(wall, segments);
        FillVertexColors(wall, segments);

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
                double from = percents[p];
                double to = percents[p + 1];

                Vector3 min = wall.SplineInstance.EvaluatePosition(from).MMSetY(wall.transform.position.y);
                Vector3 max = wall.SplineInstance.EvaluatePosition(to).MMSetY(wall.transform.position.y);


                min += Vector3.up * (float)heights[h];
                max += Vector3.up * (float)heights[h + 1];

                min = wall.transform.InverseTransformPoint(min);
                max = wall.transform.InverseTransformPoint(max);

                bool lengthValid = (max.MMSetY(min.y) - min).magnitude > 0f; 
                bool heightValid = (min.MMSetY(max.y) - min).magnitude > 0f; 
                
                if (lengthValid && heightValid)
                {
                    WallSegment segment = new WallSegment(min, max)
                    {
                        PercentFrom = from,
                        PercentTo = to
                    };

                    segments.Add(segment);
                }
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

    private static void FillOffsets(SplineWall wall, IReadOnlyCollection<WallSegment> segments)
    {
        foreach (WallSegment segment in segments)
        {
            Vector3 dirDefault = wall.transform.TransformPoint(segment.Corners[3]) - wall.transform.TransformPoint(segment.Corners[0]);
            dirDefault = new Vector3(dirDefault.z, dirDefault.y, -dirDefault.x).normalized;
            dirDefault *= wall.Config.Width * 0.5f;
            
            Plane plane = new Plane(
                wall.transform.TransformPoint(segment.Corners[0]) + dirDefault, 
                wall.transform.TransformPoint(segment.Corners[1]) + dirDefault, 
                wall.transform.TransformPoint(segment.Corners[3]) + dirDefault);
            
            Vector3 directionFrom = WallPointUtilities.GetDirectionByPercent(wall, segment.PercentFrom);
            Vector3 directionTo = WallPointUtilities.GetDirectionByPercent(wall, segment.PercentTo);
            
            Ray rayFrom = new Ray(wall.transform.TransformPoint(segment.Corners[0]), directionFrom);
            Ray rayTo = new Ray(wall.transform.TransformPoint(segment.Corners[3]), directionTo);

            if (plane.Raycast(rayFrom, out float fromDistance))
            {
                segment.OffsetFrom = rayFrom.GetPoint(fromDistance) - rayFrom.origin;
                segment.OffsetFrom = wall.transform.InverseTransformVector(segment.OffsetFrom);
            }
            
            if (plane.Raycast(rayTo, out float toDistance))
            {
                segment.OffsetTo = rayTo.GetPoint(toDistance) - rayTo.origin;
                segment.OffsetTo = wall.transform.InverseTransformVector(segment.OffsetTo);
            }
        }
    }
    
    private static void FillWidthMultipliers(SplineWall wall, IReadOnlyCollection<WallSegment> segments)
    {
        foreach (WallSegment segment in segments)
        {
            if (TryGetSegmentRow(wall, segment, out SplineWallRow row, out double min, out double max))
            {
                segment.WidthMultiplierBottom = Mathf.Lerp(row.BottomWidthMultiplier, row.TopWidthMultiplier, 
                    (float)GetPercentBetween(segment.Min.y, (float)min, (float)max));
                
                segment.WidthMultiplierTop = Mathf.Lerp(row.BottomWidthMultiplier, row.TopWidthMultiplier, 
                    (float)GetPercentBetween(segment.Max.y, (float)min, (float)max));
            }
        }
    }
    
    private static void FillVertexColors(SplineWall wall, IReadOnlyCollection<WallSegment> segments)
    {
        foreach (WallSegment segment in segments)
        {
            if (TryGetSegmentRow(wall, segment, out SplineWallRow row, out double min, out double max))
            {
                segment.VertexColorBottom = Color.Lerp(row.VertexColorBottom, row.VertexColorTop, 
                    (float)GetPercentBetween(segment.Min.y, (float)min, (float)max));
                
                segment.VertexColorTop = Color.Lerp(row.VertexColorBottom, row.VertexColorTop, 
                    (float)GetPercentBetween(segment.Max.y, (float)min, (float)max));
            }
        }
    }

    private static bool TryGetSegmentRow(SplineWall wall, WallSegment segment, out SplineWallRow row, out double min, out double max)
    {
        double segmentCenterHeight = (segment.Min.y + segment.Max.y) * 0.5f;
        
        double rowMin = 0f;
        double rowMax = 0f;
        
        row = wall.Config.WallParts.ToList().Find(x =>
        {
            wall.Config.GetRowHeightBounds(x, out rowMin, out rowMax);
            return segmentCenterHeight >= rowMin && segmentCenterHeight <= rowMax;
        });

        min = rowMin;
        max = rowMax;
        
        return row != null;
    }

    private static float GetPercentBetween(float value, float min, float max)
    {
        return (value - min) / (max - min); 
    }
}
