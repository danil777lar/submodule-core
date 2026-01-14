using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Larje.Core.Tools;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEditor;
using UnityEngine.Rendering;

namespace Larje.Core.Services
{
    [BindService(typeof(ILevelManagerService), typeof(IGameStateService))]
    public class LevelManagerAddressablesService : Service, ILevelManagerService, IGameStateService
    {
        [SerializeField] private bool editorMode;
        [SerializeField] private bool spawnLevelOnInit;
        [SerializeField] private Transform levelHolder;
        [SerializeField, HideInInspector] private LevelOptions[] levels;

        [InjectService] private IDataService _dataService;

        private bool _firstLevelSpawn = true;
        private bool _isInstantiatingLevel;
        private LevelProcessor _currentLevel;
        private GameStateType _currentState = GameStateType.Menu;

        public bool IsLevelPlaying => _currentLevel != null && _currentLevel.IsLevelPlaying;
        public GameStateType CurrentState 
        {
            get => _currentState; 
            private set
            {
                if (_currentState != value)
                {
                    GameStateType previousState = _currentState;
                    _currentState = value;
                    EventGameStateChanged?.Invoke(previousState, _currentState);
                }
            } 
        }

        public event Action<LevelProcessor> EventLevelInstantiated;
        public event Action<GameStateType, GameStateType> EventGameStateChanged;

        public override void Init()
        {
            #if !UNITY_EDITOR
            editorMode = false;
            #endif

            if (spawnLevelOnInit)
            {
                SpawnCurrentLevel();   
            }
        }

        public async void SpawnCurrentLevel()
        {
            CurrentState = GameStateType.Menu;

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

            GetLevelHolder().DestroyAllChildren();
            int levelId = GetCurrentLevelIndex();
            _dataService.GameData.levelManagerAddressablesData.LastLevelIndex = levelId;
            _dataService.SaveGameData();
            
            GameObject levelInstance = await Addressables.InstantiateAsync(levels[levelId].LevelPrefab, GetLevelHolder()).Task;
            
            if (levelInstance != null)
            {
                if (levelInstance.TryGetComponent(out LevelProcessor level))
                {
                    _currentLevel = level;
                    _currentLevel.EventLevelStart += (data) => CurrentState = GameStateType.Playing; 
                    _currentLevel.EventLevelStop += (data) => CurrentState = data.IsWin ? GameStateType.Win : GameStateType.Fail;
                    EventLevelInstantiated?.Invoke(level);
                }
                else
                {
                    Debug.LogError("Level Service: Level component not found");
                }
            }
            else
            {
                Debug.LogError($"Level Service: Prefab with key {levels[levelId].LevelPrefab} not found");
            }

            _isInstantiatingLevel = false;
            _firstLevelSpawn = false;
        }

        public async void SpawnLevelInDebugMode(AssetReference prefab)
        {
#if UNITY_EDITOR
            GetLevelHolder().DestroyAllChildren();
            var handle = Addressables.LoadAssetAsync<GameObject>(prefab);
            while (!handle.IsDone)
            {
                await Task.Yield();
            }
            PrefabUtility.InstantiatePrefab(handle.Result, GetLevelHolder());
#endif
        }

        public void IncrementLevelId()
        {
            _dataService.GameData.levelManagerAddressablesData.CurrentLevelCount++;
            List<int> randomLevels = _dataService.GameData.levelManagerAddressablesData.RandomLevels;
            if (randomLevels != null && randomLevels.Count > 0)
            {
                randomLevels.RemoveAt(0);
            }
            _dataService.SaveGameData();
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
        
        public void TrySendEventToCurrentLevel(LevelEvent levelEvent)
        {
            if (_currentLevel != null)
            {
                _currentLevel.SendEvent(levelEvent);
            }
        }

        public T GetCurrentLevelData<T>() where T : LevelProcessor.LevelData 
        {
            if (_currentLevel.GetLevelData() is T levelData)
            {
                return levelData;   
            }
            else
            {
                return null;
            }
        }

        public bool IsInstantiatingLevel()
        {
            return _isInstantiatingLevel;
        }

        public int GetCurrentLevelCount()
        {
            return _dataService.GameData.levelManagerAddressablesData.CurrentLevelCount;
        }
        
        public int GetCurrentLevelIndex()
        {
            int id = _dataService.GameData.levelManagerAddressablesData.CurrentLevelCount;
            if (id >= levels.Length)
            {
                List<int> randomLevels = _dataService.GameData.levelManagerAddressablesData.RandomLevels;
                if (randomLevels == null || randomLevels.Count == 0)
                {
                    randomLevels = new List<int>();
                    _dataService.GameData.levelManagerAddressablesData.RandomLevels = randomLevels;
                    foreach (LevelOptions level in levels.Where(x => x.AddToRandomList))
                    {
                        randomLevels.Add(levels.ToList().IndexOf(level));
                    }
                    randomLevels.Shuffle();
                    if (randomLevels.Count > 1 && randomLevels[0] == _dataService.GameData.levelManagerAddressablesData.LastLevelIndex)
                    {
                        randomLevels.Swap(0, UnityEngine.Random.Range(1, randomLevels.Count));
                    }
                    _dataService.SaveGameData();
                }
                id = randomLevels[0];
            }
            return id;
        }
        
        public void SetCurrentLevelIndex(int id)
        {
            _dataService.GameData.levelManagerAddressablesData.CurrentLevelCount = id;
            _dataService.SaveGameData();
        }

        private Transform GetLevelHolder()
        {
            return levelHolder ? levelHolder : transform;
        }

        [Serializable]
        public class LevelOptions
        {
            [SerializeField] private bool addToRandomList = true;
            [SerializeField] private AssetReference levelPrefab;

            public bool AddToRandomList => addToRandomList;
            public AssetReference LevelPrefab => levelPrefab;
        }
    }
}
