using System;
using UnityEngine;

public class OverlayEntryFps : OverlayEntry
{
    private int frames;
    private float time;
    private float fps;

    public override string Group => OverlayEntry.GROUP_RENDERING;
    public override Func<string> GetData => GenerateData;

    private string GenerateData()
    {
        frames++;
        time += Time.unscaledDeltaTime;

        if (time >= 0.5f)
        {
            fps = frames / time;
            frames = 0;
            time = 0;
        }

        return $"FPS: {Mathf.Round(fps).ToString()}";
    }
}
