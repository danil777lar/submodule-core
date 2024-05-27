using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using Larje.Core.Tools.RoomGenerator;
using MoreMountains.Tools;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineWall))]
public class SplineWallEditor : Editor
{
    private WallSegment _selectedSegment;
    
    private void OnSceneGUI()
    {
        SplineWall wall = (SplineWall) target;

        if (!wall.EnableDebugMode)
        {
            return;
        }

        foreach (WallSegment segment in WallSegmentUtilities.GetSegments(wall))
        {
            Vector3 min = wall.transform.TransformPoint(segment.Min);
            Vector3 max = wall.transform.TransformPoint(segment.Max);
            
            Vector3 segmentPosition = Vector3.Lerp(min, max, 0.5f);
            Vector3 lookDirection = max.MMSetY(min.y) - min;
            lookDirection = new Vector3(lookDirection.z, lookDirection.y, -lookDirection.x);
            Quaternion segmentRotation = Quaternion.LookRotation(lookDirection);
            
            Handles.color = Color.white.SetAlpha(0.1f);
            if (Handles.Button(segmentPosition, segmentRotation, 0.2f, 0.4f, Handles.RectangleHandleCap))
            {
                _selectedSegment = segment;
            }
        }

        if (_selectedSegment != null)
        {
            DrawFilledSegment(wall, _selectedSegment, Color.red);
            if (_selectedSegment.Prev != null) DrawFilledSegment(wall, _selectedSegment.Prev, Color.blue);
            if (_selectedSegment.Next != null) DrawFilledSegment(wall, _selectedSegment.Next, Color.blue);
            if (_selectedSegment.Upper != null) DrawFilledSegment(wall, _selectedSegment.Upper, Color.blue);
            if (_selectedSegment.Lower != null) DrawFilledSegment(wall, _selectedSegment.Lower, Color.blue);
        }
    }

    private void DrawFilledSegment(SplineWall wall, WallSegment segment, Color color)
    {
        Handles.color = color;
        int linesCount = 100;
        for (int i = 0; i < linesCount; i++)
        {
            Vector3 min = wall.transform.TransformPoint(segment.Min);
            Vector3 max = wall.transform.TransformPoint(segment.Max);
            
            float t = (float)i / (float)linesCount;
            Vector3 a = Vector3.Lerp(min, max.MMSetY(min.y), t);
            Vector3 b = Vector3.Lerp(min.MMSetY(max.y), max, t);
            Handles.DrawLine(a, b);
        }
    }
}
