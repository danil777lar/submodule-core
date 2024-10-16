using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Larje.Core.Services
{
    [CreateAssetMenu(fileName = "Sound Service Config", menuName = "Configs/Services/Sound Service Config")]
    public class SoundServiceConfig : ScriptableObject
    {
        private const string PATH = "/Plugins/ProjectConstants/SoundConstants.cs";
        private const string NAMESPACE = "Larje.Core.Services";
        
        [SerializeField] public List<AssetReferenceGameObject> sounds;
        
        public AssetReferenceGameObject GetSound(SoundType soundType)
        {
            return sounds.Find(x => FormatName(x.editorAsset.name) == soundType.ToString());
        }

        [ContextMenu("Save")]
        private async void Save()
        {
            string fullPath = Application.dataPath + PATH;
            if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            }

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            await File.WriteAllTextAsync(fullPath, GetScriptText());
            UpdateScriptingDefinedSymbols();
            AssetDatabase.Refresh();
        }
        
        private string GetScriptText()
        {
            string script = $"using System;\nnamespace {NAMESPACE}\n{{\n";
            script += "//SCRIPT IS GENERATED AUTOMATICALLY\n";
            script += "//TO CHANGE THE CONTENT YOU SHOULD USE THE CONFIG\n";

            int index = 0;
            string constantText = $"\tpublic enum SoundType\n\t{{\n";
            string constantFlagsText = $"\t[Flags]\n\tpublic enum SoundTypes\n\t{{\n";
            foreach (AssetReferenceGameObject value in sounds)
            {
                string valueName = FormatName(value.editorAsset.name);
                constantText += $"\t\t{valueName} = 1 << {index},\n";
                constantFlagsText += $"\t\t{valueName} = 1 << {index},\n";
                index++;
            }

            constantText += "\t}\n\n";
            constantFlagsText += "\t}\n\n";
            script += constantText;
            script += constantFlagsText;

            script += "}\n";
            return script;
        }
        
        private void UpdateScriptingDefinedSymbols()
        {
            BuildTargetGroup[] targetGroups = new[]
                { BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.Standalone };
            foreach (BuildTargetGroup buildGroup in targetGroups)
            {
                List<string> allSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildGroup).Split(';').ToList();
                allSymbols = allSymbols.Where(x => !x.Contains("SOUND_SERVICE_INITIALIZED")).ToList();
                
                allSymbols.Add($"SOUND_SERVICE_INITIALIZED");

                string symbolsLine = "";
                allSymbols.ForEach(x => symbolsLine += $"{x};");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildGroup, symbolsLine);
            }
        }

        private string FormatName(string name)
        {
            return name.Replace(" ", "_"); 
        }
    }
}