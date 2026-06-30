using System;
using UnityEngine;

public abstract class OverlayEntry
{
    public const string GROUP_RENDERING = "Rendering";
    public const string GROUP_DEBUG = "Debug";

    public abstract string Group { get; }

    public abstract Func<string> GetData { get; }
}
