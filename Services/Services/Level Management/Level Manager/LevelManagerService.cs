using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using MoreMountains.Tools;
using UnityEditor;

namespace Larje.Core.Services.LevelManagement
{
    [BindService(typeof(LevelManagerService))]
    public class LevelManagerService : Service
    {
        [SerializeField] private string[] _levelKeys;
        private int _currentLevelIndex;
        private LevelProcessor _currentLevel;

        public bool InstantiatingLevel
        {
            get;
            private set;
        }


        public override void Init() { }

        public async void SpawnCurrentLevel()
        {
            InstantiatingLevel = true;

            transform.MMDestroyAllChildren();
            GameObject levelInstance = await Addressables.InstantiateAsync("Levels/" + _levelKeys[_currentLevelIndex], transform).Task;
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
                Debug.LogError($"Level Service: Prefab with key {_levelKeys[_currentLevelIndex]} not found");
            }

            InstantiatingLevel = false;
        }

        public async void SpawnLevelInDebugMode(string key)
        {
            transform.MMDestroyAllChildren();
            var handle = Addressables.LoadAssetAsync<GameObject>("Levels/" + _levelKeys[_currentLevelIndex]);
            while (!handle.IsDone)
            {
                await Task.Yield();
            }
            PrefabUtility.InstantiatePrefab(handle.Result, transform);
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
    }
}