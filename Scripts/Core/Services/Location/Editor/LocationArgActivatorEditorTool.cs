using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using ProjectConstants;

public class LocationArgActivatorEditorTool : EditorWindow
{
    private LocationArgType[] args;

    [MenuItem("Tools/Location Arg Activator Tool")]
    public static void Open()
    {
        GetWindow<LocationArgActivatorEditorTool>("Arg Activator Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Batch Set Active Option", EditorStyles.boldLabel);

        int newSize = EditorGUILayout.IntField("Size", args?.Length ?? 0);

        if (args == null || args.Length != newSize)
        {
            Array.Resize(ref args, newSize);
        }

        for (int i = 0; i < args.Length; i++)
        {
            args[i] = (LocationArgType)EditorGUILayout.EnumPopup($"Element {i}", args[i]);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Apply To All Matching Objects"))
        {
            Apply();
        }
    }

    private void Apply()
    {
        LocationArgActivator[] activators = FindObjectsByType<LocationArgActivator>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();

        foreach (LocationArgActivator activator in activators)
        {
            Undo.RecordObject(activator.gameObject, "Set LocationArgActivator Option");

            activator.OnArgReceived(args);

            EditorUtility.SetDirty(activator);
            EditorUtility.SetDirty(activator.gameObject);

            Transform[] children = activator.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                EditorUtility.SetDirty(child.gameObject);
            }
        }

        Undo.CollapseUndoOperations(undoGroup);
        EditorSceneManager.MarkAllScenesDirty();
    }
}
