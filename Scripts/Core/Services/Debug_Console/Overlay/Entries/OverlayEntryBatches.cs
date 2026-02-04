using System;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;

public class OverlayEntryBatches : OverlayEntry
{
    private ProfilerRecorder _batchesRecorder;

    public override string Group => OverlayEntry.GROUP_RENDERING;
    public override Func<string> GetData => GenerateData;

    private string GenerateData()
    {
        if (!_batchesRecorder.Valid)
        {
            _batchesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Batches Count");
        }
        return $"BATCHES: {_batchesRecorder.LastValue.ToString()}";
    }
}
