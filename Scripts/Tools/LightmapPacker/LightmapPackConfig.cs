using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
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

        string fileName = $"Lightpack_{scene.name.Replace(" ", "_").Replace(".","_")}";
        string path = UnityEditor.EditorUtility.SaveFilePanelInProject("Save LightmapSet", $"{fileName}_", "asset", "Choose where to save the LightmapSet asset");
        string dirName = $"{System.IO.Path.GetFileNameWithoutExtension(path)}_Lightmaps";
        string lmDir = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path), dirName).Replace("\\", "/");

        EnsureFolderExists(lmDir);
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        LightmapPackConfig asset = ScriptableObject.CreateInstance<LightmapPackConfig>();

        ExportLightmaps(asset, lightmaps, path, lmDir);
        ExportLightprobes(asset);

        UnityEditor.AssetDatabase.CreateAsset(asset, path);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();

        Debug.Log($"Exported LightmapSet: {path}\n:Lightmaps Count{asset.lightmaps.Length}\nMeshRenderers mapped: {asset.rendererEntries.Length}");
        UnityEditor.Selection.activeObject = asset;
    }

    private static void ExportLightmaps(LightmapPackConfig asset, LightmapData[] lightmaps, string path, string lmDir)
    {
        string packName = System.IO.Path.GetFileNameWithoutExtension(path);
        int n = lightmaps.Length;
        asset.lightmaps = new LightmapSaveData[n];

        for (int i = 0; i < n; i++)
        {
            asset.lightmaps[i].color = SaveTextureToAddressables(lightmaps[i].lightmapColor, packName, lmDir);
            asset.lightmaps[i].direction = SaveTextureToAddressables(lightmaps[i].lightmapDir, packName, lmDir);
            asset.lightmaps[i].shadow = SaveTextureToAddressables(lightmaps[i].shadowMask, packName, lmDir);
        }

        List<string> rendNames = new List<string>();
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

                if (rendNames.Contains(entry.path))
                {
                    Debug.LogError($"Duplicate renderer path: {entry.path}. Skipping.", rend);
                    continue;
                }

                rendNames.Add(entry.path);
                list.Add(entry);
            }
        }

        asset.rendererEntries = list.ToArray();
    }

    private static void ExportLightprobes(LightmapPackConfig asset)
    { 
        LightProbes probes = LightmapSettings.lightProbes;
        if (probes == null)
        {
            Debug.LogError("LightmapSettings.lightProbes is null.");
            return;
        }

        SphericalHarmonicsL2[] baked = probes.bakedProbes;
        if (baked == null || baked.Length == 0)
        {
            Debug.LogError("No baked probes found.");
            return;
        }

        Vector3[] positions = probes.GetPositionsSelf();
        if (positions == null || positions.Length != baked.Length)
        {
            Debug.LogError("Positions count does not match baked probes count.");
            return;
        }

        asset.lightprobes = new LightprobeSaveData[baked.Length];

        for (int i = 0; i < baked.Length; i++)
        {
            asset.lightprobes[i] = new LightprobeSaveData
            {
                position = positions[i],
                coefficients = SerializeSH(baked[i])
            };
        }
    }   

    private static float[] SerializeSH(SphericalHarmonicsL2 sh)
    {
        float[] data = new float[27];
        int k = 0;

        for (int rgb = 0; rgb < 3; rgb++)
        {
            for (int coef = 0; coef < 9; coef++)
            {
                data[k++] = sh[rgb, coef];
            }
        }

        return data;
    }

    private static string SaveTextureToAddressables(Texture2D src, string packName, string targetDir)
    {
        string address = "";

        if (src == null)
        {
            return null;
        }

        string srcPath = UnityEditor.AssetDatabase.GetAssetPath(src);
        if (string.IsNullOrEmpty(srcPath))
        {
            Debug.LogWarning($"Texture '{src.name}' has no asset path (not an asset?). Skipping copy.");
            return address;
        }

        string ext = System.IO.Path.GetExtension(srcPath);
        if (string.IsNullOrEmpty(ext))
        {
            ext = ".asset";
        }

        string fullPath = System.IO.Path.Combine(targetDir, $"{packName}_{src.name}{ext}");
        string targetPath = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(fullPath);

        bool ok = UnityEditor.AssetDatabase.CopyAsset(srcPath, targetPath);
        if (!ok)
        {
            Debug.LogWarning($"Failed to copy '{srcPath}' -> '{targetPath}'. Using original reference.");
            return address;
        }

        UnityEditor.AssetDatabase.ImportAsset(targetPath);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();

        string guid = UnityEditor.AssetDatabase.AssetPathToGUID(targetPath);
        if (string.IsNullOrEmpty(guid))
        {
            Debug.LogError($"Failed to get GUID for copied asset: {targetPath}");
            return null;
        }

        UnityEditor.AddressableAssets.Settings.AddressableAssetSettings settings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("Addressables settings not found.");
            return null;
        }

        UnityEditor.AddressableAssets.Settings.AddressableAssetGroup group = GetOrCreateGroup(packName);
        if (group == null)
        {
            Debug.LogError("Default Addressables group not found.");
            return null;
        }

        UnityEditor.AddressableAssets.Settings.AddressableAssetEntry entry = settings.FindAssetEntry(guid);
        if (entry == null)
        {
            entry = settings.CreateOrMoveEntry(guid, group);
        }
        else if (entry.parentGroup != group)
        {
            settings.MoveEntry(entry, group);
        }

        address = System.IO.Path.GetFileNameWithoutExtension(targetPath);
        entry.address = address;

        UnityEditor.EditorUtility.SetDirty(settings);
        UnityEditor.AssetDatabase.SaveAssets();

        return address;
    }

    private static UnityEditor.AddressableAssets.Settings.AddressableAssetGroup GetOrCreateGroup(string groupName)
    {
        UnityEditor.AddressableAssets.Settings.AddressableAssetSettings settings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("Addressables settings not found.");
            return null;
        }

        UnityEditor.AddressableAssets.Settings.AddressableAssetGroup group = settings.FindGroup(groupName);
        if (group != null)
        {
            return group;
        }

        group = settings.CreateGroup(groupName, false, false, true, null, 
            typeof(UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema),
            typeof(UnityEditor.AddressableAssets.Settings.GroupSchemas.ContentUpdateGroupSchema));

        var bundledSchema = group.GetSchema<UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema>();
        if (bundledSchema != null)
        {
            bundledSchema.BundleMode = UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema.BundlePackingMode.PackTogether;
        }

        UnityEditor.EditorUtility.SetDirty(settings);
        UnityEditor.AssetDatabase.SaveAssets();

        return group;
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


    [SerializeField] private LightmapSaveData[] lightmaps;
    [SerializeField] private LightprobeSaveData[] lightprobes;
    [SerializeField] private RendererEntry[] rendererEntries;

    [ContextMenu("Apply")]
    public async void Apply()
    {
        LightmapData[] lightmaps = await LoadLightmaps();
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

        ApplyLightprobes();
    }

    public async Task<LightmapData[]> LoadLightmaps()
    {
        int n = lightmaps.Length;
        LightmapData[] arr = new LightmapData[n];

        for (int i = 0; i < n; i++)
        {
            LightmapSaveData lm = lightmaps[i];

            Texture2D color = null;
            Texture2D dir = null;
            Texture2D mask = null;

            if (!string.IsNullOrEmpty(lm.color))
            {
                color = await Addressables.LoadAssetAsync<Texture2D>(lm.color).Task;
            }

            if (!string.IsNullOrEmpty(lm.direction))
            {
                dir = await Addressables.LoadAssetAsync<Texture2D>(lm.direction).Task;
            }

            if (!string.IsNullOrEmpty(lm.shadow))
            {
                mask = await Addressables.LoadAssetAsync<Texture2D>(lm.shadow).Task;
            }

            LightmapData d = new LightmapData();
            d.lightmapColor = color;
            d.lightmapDir = dir;
            d.shadowMask = mask;

            arr[i] = d;
        }

        return arr;
    }

    public bool ApplyLightprobes()
    {
        if (lightprobes == null || lightprobes.Length == 0)
        {
            Debug.LogError("FullLightProbesData is empty.");
            return false;
        }

        Scene scene = SceneManager.GetActiveScene();

        LightProbes probes = LightProbes.GetInstantiatedLightProbesForScene(scene);
        if (probes == null)
        {
            Debug.LogError("Failed to get instantiated LightProbes for scene.");
            return false;
        }

        Vector3[] positions = new Vector3[lightprobes.Length];
        SphericalHarmonicsL2[] baked = new SphericalHarmonicsL2[lightprobes.Length];

        for (int i = 0; i < lightprobes.Length; i++)
        {
            positions[i] = lightprobes[i].position;
            baked[i] = DeserializeSH(lightprobes[i].coefficients);
        }

        probes.SetPositionsSelf(positions, true);
        probes.bakedProbes = baked;

        LightProbes.Tetrahedralize();

        return true;
    }

    private SphericalHarmonicsL2 DeserializeSH(float[] data)
    {
        SphericalHarmonicsL2 sh = new SphericalHarmonicsL2();

        if (data == null || data.Length != 27)
        {
            Debug.LogError("Invalid SH coefficient array.");
            return sh;
        }

        int k = 0;
        for (int rgb = 0; rgb < 3; rgb++)
        {
            for (int coef = 0; coef < 9; coef++)
            {
                sh[rgb, coef] = data[k++];
            }
        }

        return sh;
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
    public struct LightmapSaveData
    {
        public string color;
        public string direction;
        public string shadow;
    }    

    [Serializable]
    public class LightprobeSaveData
    {
        public Vector3 position;
        public float[] coefficients = new float[27];
    }

    [Serializable]
    public struct RendererEntry
    {
        public string path;
        public int lightmapIndex;
        public Vector4 scaleOffset;
    }   
}
