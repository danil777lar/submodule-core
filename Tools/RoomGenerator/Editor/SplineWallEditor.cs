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
    private SplineWall.WallSegment _selectedSegment;
    
    private void OnSceneGUI()
    {
        SplineWall wall = (SplineWall) target;
        SplineComputer spline = wall.SplineInstance;
        
        foreach (SplineWall.WallSegment segment in wall.GetSegments())
        {
            Vector3 segmentPosition = Vector3.Lerp(segment.Min, segment.Max, 0.5f);
            Vector3 lookDirection = segment.Max.MMSetY(segment.Min.y) - segment.Min;
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
            DrawFilledSegment(_selectedSegment, Color.red);
            if (_selectedSegment.Prev != null) DrawFilledSegment(_selectedSegment.Prev, Color.blue);
            if (_selectedSegment.Next != null) DrawFilledSegment(_selectedSegment.Next, Color.blue);
            if (_selectedSegment.Upper != null) DrawFilledSegment(_selectedSegment.Upper, Color.blue);
            if (_selectedSegment.Lower != null) DrawFilledSegment(_selectedSegment.Lower, Color.blue);
        }
    }

    private void DrawFilledSegment(SplineWall.WallSegment segment, Color color)
    {
        Handles.color = color;
        int linesCount = 100;
        for (int i = 0; i < linesCount; i++)
        {
            float t = (float)i / (float)linesCount;
            Vector3 a = Vector3.Lerp(segment.Min, segment.Max.MMSetY(segment.Min.y), t);
            Vector3 b = Vector3.Lerp(segment.Min.MMSetY(segment.Max.y), segment.Max, t);
            Handles.DrawLine(a, b);
        }
    }
}
