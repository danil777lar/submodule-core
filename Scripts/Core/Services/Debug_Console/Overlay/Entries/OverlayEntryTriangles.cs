using System;
using Unity.Profiling;
using UnityEngine;

public class OverlayEntryTriangles : OverlayEntry
{
    private ProfilerRecorder _trianglesRecorder;

    public override string Group => OverlayEntry.GROUP_RENDERING;

    public override Func<string> GetData => GenerateData;

    private string GenerateData()
    {
        if (!_trianglesRecorder.Valid)
        {
            _trianglesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count");
        }

        int trianglesCount = int.Parse(_trianglesRecorder.LastValue.ToString());
        string result = trianglesCount.ToString();
        if (trianglesCount >= 1000000)
        {
            result = (trianglesCount / 1000000f).ToString("F2") + "M";
        }
        else if (trianglesCount >= 1000)
        {
            result = (trianglesCount / 1000f).ToString("F2") + "K";
        }
        return $"TRIANGLES: {result}";
    }
}
