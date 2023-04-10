using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using MoreMountains.Tools;
using UnityEditor;
using Larje.Core.Services;

namespace Larje.Core.Services
{
    [BindService(typeof(ILevelManagerService))]
    public class LevelManagerAddressablesService : Service, ILevelManagerService
    {
        [SerializeField] private bool editorMode;
        [SerializeField] private Transform levelHolder;
        [SerializeField, HideInInspector] private LevelOptions[] levels;
        
        [InjectService] private DataService _dataService;

        private bool _firstLevelSpawn = true;
        private bool _isInstantiatingLevel;
        private LevelProcessor _currentLevel;

        public override void Init()
        {
            #if !UNITY_EDITOR
            editorMode = false;
            #endif
        }

        public async void SpawnCurrentLevel()
        {
            if (editorMode && _firstLevelSpawn)
            {
                _currentLevel = GetLevelHolder().GetComponentInChildren<LevelProcessor>();
                if (_currentLevel != null)
                {
                    _firstLevelSpawn = false;
                    return;
                }
            }

            _isInstantiatingLevel = true;

            GetLevelHolder().MMDestroyAllChildren();
            int levelId = GetCurrentLevelIndex();
            _dataService.Data.levelManagerAddressablesData.LastLevelIndex = levelId;
            _dataService.Save();
            
            GameObject levelInstance = await Addressables.InstantiateAsync("Levels/" + levels[levelId].Key, GetLevelHolder()).Task;
            if (levelInstance != null)
            {
                if (levelInstance.TryGetComponent(out LevelProcessor level))
                {
                    _currentLevel = level;
                }
                else
                {
                    Debug.LogError("Level Service: Level component not found");
                }
            }
            else
            {
                Debug.LogError($"Level Service: Prefab with key {levels[levelId].Key} not found");
            }

            _isInstantiatingLevel = false;
            _firstLevelSpawn = false;
        }

        public async void SpawnLevelInDebugMode(string key)
        {
#if UNITY_EDITOR
            GetLevelHolder().MMDestroyAllChildren();
            var handle = Addressables.LoadAssetAsync<GameObject>($"Levels/{key}");
            while (!handle.IsDone)
            {
                await Task.Yield();
            }
            PrefabUtility.InstantiatePrefab(handle.Result, GetLevelHolder());
#endif
        }

        public void IncrementLevelId()
        {
            _dataService.Data.levelManagerAddressablesData.CurrentLevelCount++;
            List<int> randomLevels = _dataService.Data.levelManagerAddressablesData.RandomLevels;
            if (randomLevels != null && randomLevels.Count > 0)
            {
                randomLevels.RemoveAt(0);
            }
            _dataService.Save();
        }

        public void TryStartCurrentLevel(LevelProcessor.StartData data)
        {
            if (_currentLevel != null)
            {
                _currentLevel.TryStartLevel(data);
            }
        }

        public void TryStopCurrentLevel(LevelProcessor.StopData data)
        {
            if (_currentLevel != null)
            {
                _currentLevel.TryStopLevel(data);
            }
        }

        public bool IsInstantiatingLevel()
        {
            return _isInstantiatingLevel;
        }

        public float GetCurrentLevelProgress()
        {
            return _currentLevel.GetLevelProgress();
        }

        public int GetCurrentLevelCount()
        {
            return _dataService.Data.levelManagerAddressablesData.CurrentLevelCount;
        }
        
        public int GetCurrentLevelIndex()
        {
            int id = _dataService.Data.levelManagerAddressablesData.CurrentLevelCount;
            if (id >= levels.Length)
            {
                List<int> randomLevels = _dataService.Data.levelManagerAddressablesData.RandomLevels;
                if (randomLevels == null || randomLevels.Count == 0)
                {
                    randomLevels = new List<int>();
                    _dataService.Data.levelManagerAddressablesData.RandomLevels = randomLevels;
                    foreach (LevelOptions level in levels.Where(x => x.AddToRandomList))
                    {
                        randomLevels.Add(levels.ToList().IndexOf(level));
                    }
                    randomLevels.MMShuffle();
                    if (randomLevels.Count > 1 && randomLevels[0] == _dataService.Data.levelManagerAddressablesData.LastLevelIndex)
                    {
                        randomLevels.MMSwap(0, UnityEngine.Random.Range(1, randomLevels.Count));
                    }
                    _dataService.Save();
                }
                id = randomLevels[0];
            }
            return id;
        }

        private Transform GetLevelHolder()
        {
            return levelHolder ? levelHolder : transform;
        }

        [Serializable]
        public class LevelOptions
        {
            [SerializeField] private bool addToRandomList = true;
            [SerializeField] private string key = "";

            public bool AddToRandomList => addToRandomList;
            public string Key => key;
        }
    }
}