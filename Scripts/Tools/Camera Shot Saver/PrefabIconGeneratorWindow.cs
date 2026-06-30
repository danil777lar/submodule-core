using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public class PrefabIconGeneratorWindow : EditorWindow
{
    private int _width = 256;
    private int _height = 256;
    private Vector3 _cameraAngle = new Vector3(30f, -45f, 0f);
    private float _padding = 0.15f;
    private bool _transparentBackground = true;
    private Color _backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    private string _outputFolder = "Assets/Icons";
    private string _filePrefix = "";
    private bool _addToAddressables = false;
    private string _addressablesGroup = "";

    private Vector2 _scroll;

    [MenuItem("Tools/Prefab Icon Generator")]
    public static void Open() => GetWindow<PrefabIconGeneratorWindow>("Prefab Icon Generator");

    private void OnGUI()
    {
        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
        using (new EditorGUILayout.HorizontalScope())
        {
            _outputFolder = EditorGUILayout.TextField("Folder", _outputFolder);
            if (GUILayout.Button("...", GUILayout.Width(28)))
            {
                string picked = EditorUtility.OpenFolderPanel("Select output folder", "Assets", "");
                if (!string.IsNullOrEmpty(picked))
                {
                    string projectPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
                    if (picked.StartsWith(projectPath))
                        _outputFolder = "Assets" + picked.Substring(projectPath.Length).Replace('\\', '/');
                }
            }
        }
        _filePrefix = EditorGUILayout.TextField("File Prefix", _filePrefix);

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Resolution", EditorStyles.boldLabel);
        _width  = EditorGUILayout.IntField("Width",  Mathf.Max(1, _width));
        _height = EditorGUILayout.IntField("Height", Mathf.Max(1, _height));
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("64"))  { _width = _height = 64;  }
            if (GUILayout.Button("128")) { _width = _height = 128; }
            if (GUILayout.Button("256")) { _width = _height = 256; }
            if (GUILayout.Button("512")) { _width = _height = 512; }
        }

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Camera", EditorStyles.boldLabel);
        _cameraAngle = EditorGUILayout.Vector3Field("Angle  (X=Pitch  Y=Yaw  Z=Roll)", _cameraAngle);
        _padding = EditorGUILayout.Slider("Padding", _padding, 0f, 0.5f);
        EditorGUILayout.HelpBox(
            "Presets: isometric ≈ (30, -45, 0)   top-down ≈ (90, 0, 0)   front ≈ (0, 0, 0)",
            MessageType.None);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Isometric"))  _cameraAngle = new Vector3(30f, -45f, 0f);
            if (GUILayout.Button("Top-Down"))   _cameraAngle = new Vector3(90f,   0f, 0f);
            if (GUILayout.Button("Front"))      _cameraAngle = new Vector3( 0f,   0f, 0f);
            if (GUILayout.Button("Side"))       _cameraAngle = new Vector3( 0f, -90f, 0f);
        }

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Background", EditorStyles.boldLabel);
        _transparentBackground = EditorGUILayout.Toggle("Transparent", _transparentBackground);
        using (new EditorGUI.DisabledScope(_transparentBackground))
            _backgroundColor = EditorGUILayout.ColorField("Color", _backgroundColor);

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Addressables", EditorStyles.boldLabel);
        _addToAddressables = EditorGUILayout.Toggle("Add to Addressables", _addToAddressables);
        using (new EditorGUI.DisabledScope(!_addToAddressables))
        {
            _addressablesGroup = EditorGUILayout.TextField("Group (empty = default)", _addressablesGroup);
            EditorGUILayout.HelpBox("Address will be set to the filename without extension.", MessageType.None);
        }

        EditorGUILayout.Space(10);

        var prefabs = GetSelectedPrefabs();

        using (new EditorGUI.DisabledScope(prefabs.Count == 0))
        {
            if (GUILayout.Button($"Generate Icons  ({prefabs.Count} prefab(s) selected)", GUILayout.Height(36)))
                GenerateIcons(prefabs);
        }

        if (prefabs.Count == 0)
            EditorGUILayout.HelpBox("Select one or more prefab assets in the Project window.", MessageType.Info);
        else
        {
            EditorGUILayout.HelpBox(
                string.Join("\n", prefabs.ConvertAll(p => "• " + p.name)),
                MessageType.None);
        }

        EditorGUILayout.EndScrollView();
    }

    private List<GameObject> GetSelectedPrefabs()
    {
        var result = new List<GameObject>();
        foreach (var obj in Selection.objects)
        {
            if (obj is GameObject go && PrefabUtility.IsPartOfPrefabAsset(go))
                result.Add(go);
        }
        return result;
    }

    private void GenerateIcons(List<GameObject> prefabs)
    {
        EnsureFolder(_outputFolder);

        var saved = new List<string>();
        int done = 0;
        foreach (var prefab in prefabs)
        {
            EditorUtility.DisplayProgressBar("Generating Icons", prefab.name, (float)done / prefabs.Count);
            try
            {
                string path = GenerateIcon(prefab);
                if (path != null) saved.Add(path);
                done++;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[PrefabIconGenerator] {prefab.name}: {e}");
            }
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();

        foreach (var path in saved)
        {
            ApplySpriteImportSettings(path);
            if (_addToAddressables)
                AddToAddressables(path, _addressablesGroup);
        }

        ShowNotification(new GUIContent($"Done — {done} icon(s) → {_outputFolder}"));
    }

    private static void ApplySpriteImportSettings(string assetPath)
    {
        var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer == null) return;

        importer.textureType       = TextureImporterType.Sprite;
        importer.spriteImportMode  = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.SaveAndReimport();
    }

    private string GenerateIcon(GameObject prefab)
    {
        // Render texture
        var rt = new RenderTexture(_width, _height, 24, RenderTextureFormat.ARGB32)
        {
            antiAliasing = 4
        };
        rt.Create();

        // Camera
        var camGo = EditorUtility.CreateGameObjectWithHideFlags(
            "__IconCamera__", HideFlags.HideAndDontSave);
        var cam = camGo.AddComponent<Camera>();
        cam.clearFlags      = CameraClearFlags.SolidColor;
        cam.backgroundColor = _transparentBackground ? new Color(0, 0, 0, 0) : _backgroundColor;
        cam.targetTexture   = rt;
        cam.fieldOfView     = 30f;
        cam.nearClipPlane   = 0.001f;
        cam.farClipPlane    = 10000f;
        cam.enabled         = false;

        // Directional light
        var lightGo = EditorUtility.CreateGameObjectWithHideFlags(
            "__IconLight__", HideFlags.HideAndDontSave);
        var dirLight = lightGo.AddComponent<Light>();
        dirLight.type      = LightType.Directional;
        dirLight.intensity = 1.2f;
        dirLight.color     = Color.white;
        lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        // Instantiate prefab
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.hideFlags = HideFlags.HideAndDontSave;
        instance.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        try
        {
            var bounds = CalculateBounds(instance);

            // Position camera
            var rotation  = Quaternion.Euler(_cameraAngle);
            var forward   = rotation * Vector3.forward;
            float radius  = bounds.extents.magnitude;
            float halfFov = cam.fieldOfView * 0.5f * Mathf.Deg2Rad;
            float distance = (radius * (1f + _padding)) / Mathf.Sin(halfFov);

            camGo.transform.position = bounds.center - forward * distance;
            camGo.transform.rotation = rotation;

            // Render
            var prevActive = RenderTexture.active;
            cam.Render();

            // Read back
            RenderTexture.active = rt;
            var tex = new Texture2D(_width, _height, TextureFormat.ARGB32, false);
            tex.ReadPixels(new Rect(0, 0, _width, _height), 0, 0);
            tex.Apply();
            RenderTexture.active = prevActive;

            // Save
            byte[] png = tex.EncodeToPNG();
            string assetPath = $"{_outputFolder}/{_filePrefix}{prefab.name}.png";
            string fullPath  = Path.GetFullPath(Path.Combine(Application.dataPath, "..", assetPath));
            File.WriteAllBytes(fullPath, png);

            DestroyImmediate(tex);
            return assetPath;
        }
        finally
        {
            DestroyImmediate(instance);
            DestroyImmediate(camGo);
            DestroyImmediate(lightGo);
            rt.Release();
            DestroyImmediate(rt);
        }
    }

    private static Bounds CalculateBounds(GameObject go)
    {
        var renderers = go.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0)
            return new Bounds(go.transform.position, Vector3.one);

        var bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);

        return bounds;
    }

    private static void AddToAddressables(string assetPath, string groupName)
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogWarning("[PrefabIconGenerator] Addressables settings not found — skipping.");
            return;
        }

        AddressableAssetGroup group;
        if (string.IsNullOrWhiteSpace(groupName))
        {
            group = settings.DefaultGroup;
        }
        else
        {
            group = settings.FindGroup(groupName);
            if (group == null)
            {
                Debug.LogWarning($"[PrefabIconGenerator] Group '{groupName}' not found, using default.");
                group = settings.DefaultGroup;
            }
        }

        string guid  = AssetDatabase.AssetPathToGUID(assetPath);
        var entry    = settings.CreateOrMoveEntry(guid, group, readOnly: false, postEvent: false);
        entry.address = Path.GetFileNameWithoutExtension(assetPath);
        settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);
    }

    private static void EnsureFolder(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath))
            return;

        var parts   = folderPath.Split('/');
        string current = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);
            current = next;
        }
    }
}
