using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TransformClipboardWindow : EditorWindow
{
    private enum CopySpace { Local, World }
    private enum PasteMatch { ByOrder, ByName }

    [Serializable]
    private struct TransformData
    {
        public string name;
        public Vector3 localPos;
        public Quaternion localRot;
        public Vector3 localScale;

        public Vector3 worldPos;
        public Quaternion worldRot;
    }

    private CopySpace copySpace = CopySpace.Local;
    private PasteMatch pasteMatch = PasteMatch.ByOrder;

    private bool pastePosition = true;
    private bool pasteRotation = true;
    private bool pasteScale = true;

    private readonly List<TransformData> buffer = new();
    private readonly Dictionary<string, TransformData> bufferByName = new();

    [MenuItem("Tools/Transform Clipboard")]
    public static void Open() => GetWindow<TransformClipboardWindow>("Transform Clipboard");

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Copy", EditorStyles.boldLabel);
        copySpace = (CopySpace)EditorGUILayout.EnumPopup("Space", copySpace);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Copy From Selection", GUILayout.Height(28)))
                CopyFromSelection();

            if (GUILayout.Button("Clear", GUILayout.Height(28)))
                ClearBuffer();
        }

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Paste", EditorStyles.boldLabel);

        pasteMatch = (PasteMatch)EditorGUILayout.EnumPopup("Match", pasteMatch);

        pastePosition = EditorGUILayout.ToggleLeft("Position", pastePosition);
        pasteRotation = EditorGUILayout.ToggleLeft("Rotation", pasteRotation);

        using (new EditorGUI.DisabledScope(copySpace == CopySpace.World))
            pasteScale = EditorGUILayout.ToggleLeft("Scale (Local only)", pasteScale);

        EditorGUILayout.Space(8);

        using (new EditorGUI.DisabledScope(buffer.Count == 0))
        {
            if (GUILayout.Button("Paste To Selection", GUILayout.Height(32)))
                PasteToSelection();
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox(
            $"Buffer: {buffer.Count} item(s)\n" +
            $"CopySpace: {copySpace}, Match: {pasteMatch}",
            MessageType.Info
        );
    }

    private void CopyFromSelection()
    {
        var trs = Selection.transforms;
        if (trs == null || trs.Length == 0)
        {
            ShowNotification(new GUIContent("Nothing selected"));
            return;
        }

        buffer.Clear();
        bufferByName.Clear();

        foreach (var t in trs)
        {
            var d = new TransformData
            {
                name = t.name,

                localPos = t.localPosition,
                localRot = t.localRotation,
                localScale = t.localScale,

                worldPos = t.position,
                worldRot = t.rotation
            };

            buffer.Add(d);
            if (!bufferByName.ContainsKey(d.name))
                bufferByName.Add(d.name, d);
        }

        ShowNotification(new GUIContent($"Copied {buffer.Count}"));
    }

    private void PasteToSelection()
    {
        var targets = Selection.transforms;
        if (targets == null || targets.Length == 0)
        {
            ShowNotification(new GUIContent("Nothing selected to paste into"));
            return;
        }

        Undo.RecordObjects(targets, "Paste Transforms");

        if (pasteMatch == PasteMatch.ByOrder)
        {
            int n = Mathf.Min(targets.Length, buffer.Count);
            for (int i = 0; i < n; i++)
                Apply(targets[i], buffer[i]);
        }
        else
        {
            foreach (var t in targets)
            {
                if (bufferByName.TryGetValue(t.name, out var d))
                    Apply(t, d);
            }
        }

        foreach (var t in targets)
            EditorUtility.SetDirty(t);

        ShowNotification(new GUIContent("Pasted"));
    }

    private void Apply(Transform target, TransformData d)
    {
        if (copySpace == CopySpace.Local)
        {
            if (pastePosition) target.localPosition = d.localPos;
            if (pasteRotation) target.localRotation = d.localRot;
            if (pasteScale)    target.localScale = d.localScale;
        }
        else
        {
            if (pastePosition) target.position = d.worldPos;
            if (pasteRotation) target.rotation = d.worldRot;
        }
    }

    private void ClearBuffer()
    {
        buffer.Clear();
        bufferByName.Clear();
        ShowNotification(new GUIContent("Cleared"));
    }
}
