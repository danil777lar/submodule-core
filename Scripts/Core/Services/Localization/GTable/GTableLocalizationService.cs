using System.Collections;
using System.Collections.Generic;
using Larje.Core;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Larje.Core.Services
{
    [BindService(typeof(ILocalizationService))]
    public class GTableLocalizationService : Service, ILocalizationService
    {
        [Header("Main")] 
        [SerializeField] private bool loadFromWeb = true;
        [SerializeField] private string localizationUrl;
        [SerializeField] private string defaultLanguage = "en";
        [SerializeField] private List<string> tableNames;
        [Header("Cache")] 
        [SerializeField] private string cacheFileName = "localization_cache";
        [SerializeField] private List<TextAsset> localizationCache;
        [Header("Debug")] 
        [SerializeField] private bool debugMode;
        [SerializeField] private string debugLanguage;
        [Space] 
        [SerializeField] private List<GTableLocalization> _localization;

        private bool _noConnection;
        private string _systemLanguage;
        private UnityWebRequest _requestLocalization;

        public bool LocalizationLoaded { get; private set; }

        public override void Init()
        {
            if (localizationCache != null && localizationCache.Count > 0)
            {
                _localization = new List<GTableLocalization>();
                foreach (TextAsset cache in localizationCache)
                {
                    _localization.AddRange(DeserealizeLocalization(cache.text));   
                }
            }

            UnityEngine.Localization.LocaleIdentifier li = Application.systemLanguage;
            _systemLanguage = li.Code.ToLower();

#if UNITY_EDITOR
            if (debugMode)
            {
                _systemLanguage = debugLanguage;
            }
#endif

            Debug.Log("System language: " + _systemLanguage);

            StartCoroutine(LoadLocalization());
        }

        public string GetLocalizationValue(string key)
        {
            GTableLocalization localized = _localization?.Find(x => x.Key == key);
            if (localized == null)
            {
                return $"{key} not found";
            }

            GTableLocalizationValue value = localized.Values.Find(x => x.LanguageCode == _systemLanguage);
            if (value == null)
            {
                value = localized.Values.Find(x => x.LanguageCode == defaultLanguage);
            }

            return value.Value;
        }

        public float GetLoadingPercent()
        {
            if (_noConnection)
            {
                return 1f;
            }

            if (_requestLocalization != null)
            {
                return _requestLocalization.downloadProgress;
            }

            return 0f;
        }

        private IEnumerator LoadLocalization()
        {
            if (loadFromWeb)
            {
                string localizationJson = "";

                _localization = new List<GTableLocalization>();
                foreach (string tableName in tableNames)
                {
                    _requestLocalization = UnityWebRequest.Get($"{localizationUrl}?name={tableName}");
                    
                    yield return _requestLocalization.SendWebRequest();
                    
                    if (_requestLocalization.result == UnityWebRequest.Result.Success)
                    {
                        localizationJson = _requestLocalization.downloadHandler.text;
                        yield return null;

                        _localization.AddRange(DeserealizeLocalization(localizationJson));
                    }
                    else
                    {
                        _noConnection = true;
                    }
                }
            }
            else
            {
                _noConnection = true;
            }

            LocalizationLoaded = true;
        }

        private List<GTableLocalization> DeserealizeLocalization(string json)
        {
            List<GTableLocalization> localization = new List<GTableLocalization>();
            List<Dictionary<string, string>> dataRaw =
                JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);
            foreach (Dictionary<string, string> locRaw in dataRaw)
            {
                GTableLocalization loc = new GTableLocalization();
                if (locRaw.TryGetValue("key", out loc.Key) && loc.Key.ToLower() != "key")
                {
                    loc.Values = new List<GTableLocalizationValue>();
                    foreach (KeyValuePair<string, string> pair in locRaw)
                    {
                        if (pair.Key != "key")
                        {
                            GTableLocalizationValue value = new GTableLocalizationValue();
                            value.LanguageCode = pair.Key.ToLower();
                            value.Value = pair.Value;
                            loc.Values.Add(value);
                        }
                    }

                    localization.Add(loc);
                }
            }

            return localization;
        }

#if UNITY_EDITOR
        [ContextMenu("Cache Localization")]
        private void CacheLocalization()
        {
            localizationCache = new List<TextAsset>();
            foreach (string table in tableNames)
            {
                UnityWebRequest request = UnityWebRequest.Get($"{localizationUrl}?name={table}");
                request.SendWebRequest().completed += (x) =>
                {
                    if (request.downloadHandler.isDone)
                    {
                        string localizationJson = request.downloadHandler.text;
                        string fileName = $"{cacheFileName}_{table}.asset";
                        string fullPath = "Assets/" + fileName;

                        TextAsset foundAsset = localizationCache.Find(f => f.name == fileName); 
                        if (foundAsset != null)
                        {
                            fullPath = AssetDatabase.GetAssetPath(foundAsset);
                        }

                        TextAsset cache = new TextAsset(localizationJson);
                        AssetDatabase.CreateAsset(cache, fullPath);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        localizationCache.Add(cache);

                        Debug.Log("Localization cached!!");
                    }
                };
            }
        }
#endif
    }
}