#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Unity.CodeEditor;
using UnityEditor.ShortcutManagement;

public static class RegenerateProjectFiles
{
    [MenuItem("Tools/Regenerate C# Project Files %#r")] // Ctrl+Shift+R
    public static void Run()
    {
        var T = Type.GetType("UnityEditor.SyncVS,UnityEditor");
        var syncSolution = T.GetMethod("SyncSolution", BindingFlags.Public | BindingFlags.Static);
        syncSolution.Invoke(null, null);

        Debug.Log("Sync C# Project Files");
    }
}
#endif
