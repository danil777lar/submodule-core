using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LightmapPackConfig : ScriptableObject
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Lightmap Pack/Export Active Scene")]
    private static void ExportActiveScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (!scene.isLoaded)
        {
            Debug.LogError("Active scene is not loaded.");
            return;
        }

        LightmapData[] lightmaps = LightmapSettings.lightmaps;
        if (lightmaps == null || lightmaps.Length == 0)
        {
            Debug.LogError("No baked lightmaps found. Bake lighting first.");
            return;
        }

        string fileName = $"Lightpack_{scene.name.Replace(" ", "_").Replace(".","_")}_";
        string path = UnityEditor.EditorUtility.SaveFilePanelInProject("Save LightmapSet", fileName, "asset", "Choose where to save the LightmapSet asset");
        string dirName = $"{System.IO.Path.GetFileNameWithoutExtension(path)}_Lightmaps";
        string lmDir = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path), dirName).Replace("\\", "/");
        Debug.Log($"LMDIR {lmDir}");
        EnsureFolderExists(lmDir);

        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        LightmapPackConfig asset = ScriptableObject.CreateInstance<LightmapPackConfig>();

        int n = lightmaps.Length;
        asset.lightmapsColors = new Texture2D[n];
        asset.lightmapsDirections = new Texture2D[n];
        asset.lightmapsShadowMasks = new Texture2D[n];

        for (int i = 0; i < n; i++)
        {
            asset.lightmapsColors[i] = CopyTextureAsset(lightmaps[i].lightmapColor, lmDir);
            asset.lightmapsDirections[i] = CopyTextureAsset(lightmaps[i].lightmapDir, lmDir);
            asset.lightmapsShadowMasks[i] = CopyTextureAsset(lightmaps[i].shadowMask, lmDir);
        }

        List<RendererEntry> list = new List<RendererEntry>();
        MeshRenderer[] allSceneRenderers = UnityEngine.Object.FindObjectsOfType<MeshRenderer>(true);

        foreach (MeshRenderer rend in allSceneRenderers)
        {
            if (rend .lightmapIndex >= 0)
            {
                RendererEntry entry = new RendererEntry
                {
                    path = GetTransformPath(rend.transform),
                    lightmapIndex = rend.lightmapIndex,
                    scaleOffset = rend.lightmapScaleOffset
                };
                list.Add(entry);
            }
        }

        asset.rendererEntries = list.ToArray();

        UnityEditor.AssetDatabase.CreateAsset(asset, path);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();

        Debug.Log($"Exported LightmapSet: {path}\nLightmaps: {n}, MeshRenderers mapped: {asset.rendererEntries.Length}");
        UnityEditor.Selection.activeObject = asset;
    }

    private static Texture2D CopyTextureAsset(Texture2D src, string targetDir)
    {
        if (src == null)
        {
            return null;
        }

        string srcPath = UnityEditor.AssetDatabase.GetAssetPath(src);
        if (string.IsNullOrEmpty(srcPath))
        {
            Debug.LogWarning($"Texture '{src.name}' has no asset path (not an asset?). Skipping copy.");
            return src;
        }

        string ext = System.IO.Path.GetExtension(srcPath);
        if (string.IsNullOrEmpty(ext))
        {
            ext = ".asset";
        }

        string fullPath = System.IO.Path.Combine(targetDir, $"{src.name}{ext}");
        string targetPath = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(fullPath);

        bool ok = UnityEditor.AssetDatabase.CopyAsset(srcPath, targetPath);
        if (!ok)
        {
            Debug.LogWarning($"Failed to copy '{srcPath}' -> '{targetPath}'. Using original reference.");
            return src;
        }

        return UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(targetPath);
    }

    private static void EnsureFolderExists(string assetFolderPath)
    {
        if (UnityEditor.AssetDatabase.IsValidFolder(assetFolderPath))
        {
            return;
        }

        string[] parts = assetFolderPath.Split('/');
        if (parts.Length == 0 || parts[0] != "Assets")
        {
            throw new Exception("Folder path must start with 'Assets/'");
        }

        string current = "Assets";
        for (int i = 1; i < parts.Length; i++)
        {
            string next = $"{current}/{parts[i]}";
            if (!UnityEditor.AssetDatabase.IsValidFolder(next))
            {
                UnityEditor.AssetDatabase.CreateFolder(current, parts[i]);
            }
            current = next;
        }
    }

    private static string GetTransformPath(Transform t)
    {
        Stack<string> stack = new Stack<string>();
        while (t != null)
        {
            stack.Push(t.name);
            t = t.parent;
        }
        return string.Join("/", stack);
    }
#endif


    [SerializeField] private Texture2D[] lightmapsColors;
    [SerializeField] private Texture2D[] lightmapsDirections;
    [SerializeField] private Texture2D[] lightmapsShadowMasks;
    [Space]
    [SerializeField] private RendererEntry[] rendererEntries;

    [ContextMenu("Apply")]
    public void Apply()
    {
        LightmapData[] lightmaps = BuildLightmapDataArray();
        LightmapSettings.lightmaps = lightmaps;

        foreach (RendererEntry entry in rendererEntries)
        {
            Transform t = FindTransformByPath(entry.path);
            if (t != null)
            {
                MeshRenderer rend = t.GetComponent<MeshRenderer>();
                if (rend != null)
                {
                    rend.lightmapIndex = entry.lightmapIndex;
                    rend.lightmapScaleOffset = entry.scaleOffset;
                }
            }
        }
    }

    private LightmapData[] BuildLightmapDataArray()
    {
        int n = lightmapsColors != null ? lightmapsColors.Length : 0;
        LightmapData[] arr = new LightmapData[n];

        for (int i = 0; i < n; i++)
        {
            LightmapData d = new LightmapData();
            d.lightmapColor = lightmapsColors[i];

            if (lightmapsDirections != null && i < lightmapsDirections.Length)
            {
                d.lightmapDir = lightmapsDirections[i];
            }

            if (lightmapsShadowMasks != null && i < lightmapsShadowMasks.Length)
            {
                d.shadowMask = lightmapsShadowMasks[i];
            }

            arr[i] = d;
        }
        return arr;
    }

    private Transform FindTransformByPath(string path, Transform root = null)
    {
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }

        string[] parts = path.Split('/');

        Transform current = null;
        if (root != null)
        {
            if (root.name != parts[0])
            {
                return null;
            }

            current = root;
            for (int i = 1; i < parts.Length; i++)
            {
                current = current.Find(parts[i]);
                if (current == null)
                {
                    return null;
                }
            }
            return current;
        }

        Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        GameObject[] roots = scene.GetRootGameObjects();

        foreach (GameObject go in roots)
        {
            if (go.name != parts[0])
            {
                continue;
            }

            current = go.transform;
            bool ok = true;

            for (int i = 1; i < parts.Length; i++)
            {
                current = current.Find(parts[i]);
                if (current == null)
                {
                    ok = false;
                    break;
                }
            }

            if (ok)
            {
                return current;
            }
        }

        return null;
    }

    [Serializable]
    public struct RendererEntry
    {
        public string path;
        public int lightmapIndex;
        public Vector4 scaleOffset;
    }   
}
