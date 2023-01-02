using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using MoreMountains.Tools;
using Unity.VisualScripting;
using UnityEditor;
using Random = System.Random;

namespace Larje.Core.Services.LevelManagement
{
    [BindService(typeof(LevelManagerService))]
    public class LevelManagerService : Service
    {
        [SerializeField] private bool editorMode;
        [SerializeField] private Transform levelHolder;
        [SerializeField, HideInInspector] private LevelOptions[] levels;
        [InjectService] private DataService _dataService;

        private bool _firstLevelSpawn = true;
        private LevelProcessor _currentLevel;

        public bool InstantiatingLevel
        {
            get;
            private set;
        }
        public int CurrentLevelCount => _dataService.Data.LevelManagerData.CurrentLevelCount;
        public int CurrentLevelIndex => GetLevelId();

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

            InstantiatingLevel = true;

            GetLevelHolder().MMDestroyAllChildren();
            int levelId = GetLevelId();
            _dataService.Data.LevelManagerData.LastLevelIndex = levelId;
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

            InstantiatingLevel = false;
            _firstLevelSpawn = false;
        }

        public void IncrementLevelId()
        {
            _dataService.Data.LevelManagerData.CurrentLevelCount++;
            List<int> randomLevels = _dataService.Data.LevelManagerData.RandomLevels;
            if (randomLevels != null && randomLevels.Count > 0)
            {
                randomLevels.RemoveAt(0);
            }
            _dataService.Save();
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

        private int GetLevelId()
        {
            int id = _dataService.Data.LevelManagerData.CurrentLevelCount;
            if (id >= levels.Length)
            {
                List<int> randomLevels = _dataService.Data.LevelManagerData.RandomLevels;
                if (randomLevels == null || randomLevels.Count == 0)
                {
                    randomLevels = new List<int>();
                    _dataService.Data.LevelManagerData.RandomLevels = randomLevels;
                    foreach (LevelOptions level in levels.Where(x => x.AddToRandomList))
                    {
                        randomLevels.Add(levels.ToList().IndexOf(level));
                    }
                    randomLevels.MMShuffle();
                    if (randomLevels.Count > 1 && randomLevels[0] == _dataService.Data.LevelManagerData.LastLevelIndex)
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