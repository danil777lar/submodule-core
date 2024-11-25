using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Larje.Core.Services
{
    [CreateAssetMenu(fileName = "Sound Service Config", menuName = "Larje/Core/Services/Sound Service Config")]
    public class SoundServiceConfig : ScriptableObject
    {
        private const string NAMESPACE = "Larje.Core.Services";
        private const string FILE_NAME = "SoundType";
        private const string SYMBOL_PREFIX = "SOUND_SERVICE_INITIALIZED";
        
        [SerializeField] private List<AssetReference> sounds;
        
        public AssetReference GetSound(SoundType soundType)
        {
            return sounds.Find(x => FormatName(x.Asset.name) == soundType.ToString());
        }

        [ContextMenu("Save")]
        private void Save()
        {
            string[] soundTypes = sounds.Select((x) => FormatName(x.Asset.name)).ToArray();  
            
            EnumScriptBuilder builder = new EnumScriptBuilder(NAMESPACE, FILE_NAME, SYMBOL_PREFIX);
            builder.AddConstant(FILE_NAME, true, soundTypes);
            builder.Save();
        }

        private string FormatName(string name)
        {
            return name.Replace(" ", "_"); 
        }
    }
}