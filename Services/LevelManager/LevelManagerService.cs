using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

namespace Larje.Core.Services
{
    [BindService(typeof(LevelManagerService))]
    public class LevelManagerService : Service
    {
        const string LEVEL_COUNT = "CurrentLevelCount";
        const string LEVEL_ID = "LastLevelIndex";

        public bool editorMode = false;
        public int CurrentLevelCount => PlayerPrefs.GetInt(LEVEL_COUNT, 0) + 1;
        public int CurrentLevelIndex;
        public bool IsRandomOrder { get; private set; }
        public List<Level> Levels = new List<Level>();
        public Action OnLevelStarted;
        public Action OnLevelRestarted;
        public Action OnLevelCompleted;


        public void Awake()
        {
#if !UNITY_EDITOR
        editorMode = false;
#endif
            if (!editorMode) SelectLevel(PlayerPrefs.GetInt(LEVEL_ID), true);
        }

        private void OnDestroy()
        {
            PlayerPrefs.SetInt(LEVEL_ID, CurrentLevelIndex);
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.SetInt(LEVEL_ID, CurrentLevelIndex);
        }

        public override void Init() 
        {

        }

        public void StartLevel()
        {
            SendStart();
        }

        public void RestartLevel()
        {
            SendRestart();
            SelectLevel(CurrentLevelIndex, false);
        }

        public void NextLevel()
        {
            SendComplete();
            if (!editorMode)
                PlayerPrefs.SetInt(LEVEL_COUNT, (PlayerPrefs.GetInt(LEVEL_COUNT) + 1));
            SelectLevel(CurrentLevelIndex + 1);
        }

        public void ClearListAtIndex(int levelIndex)
        {
            Levels[levelIndex].LevelPrefab = null;
        }

        public void SelectLevel(int levelIndex, bool indexCheck = true)
        {
            if (indexCheck)
                levelIndex = GetCorrectedIndex(levelIndex);

            if (Levels[levelIndex].LevelPrefab == null)
            {
                Debug.Log("<color=red>There is no prefab attached!</color>");
                return;
            }

            var level = Levels[levelIndex];

            if (level.LevelPrefab)
            {
                SelLevelParams(level);
                CurrentLevelIndex = levelIndex;
            }
        }

        public void PrevLevel() =>
            SelectLevel(CurrentLevelIndex - 1);

        private int GetCorrectedIndex(int levelIndex)
        {
            if (editorMode)
                return levelIndex > Levels.Count - 1 || levelIndex <= 0 ? 0 : levelIndex;
            else
            {
                int levelId = PlayerPrefs.GetInt(LEVEL_COUNT);
                IsRandomOrder = false;
                if (levelId > Levels.Count - 1)
                {
                    IsRandomOrder = true;
                    if (Levels.Count > 1)
                    {
                        while (true)
                        {
                            levelId = UnityEngine.Random.Range(0, Levels.Count);
                            if (levelId != CurrentLevelIndex) return levelId;
                        }
                    }
                    else return UnityEngine.Random.Range(0, Levels.Count);
                }
                return levelId;
            }
        }

        private void SelLevelParams(Level level)
        {
            if (level.LevelPrefab)
            {
                ClearChilds();
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    Instantiate(level.LevelPrefab, transform);
                }
                else PrefabUtility.InstantiatePrefab(level.LevelPrefab, transform);
                foreach (IEditorModeSpawn child in GetComponentsInChildren<IEditorModeSpawn>())
                    child.EditorModeSpawn();
#else
            Instantiate(level.LevelPrefab, transform);
#endif
            }

            if (level.SkyboxMaterial)
            {
                RenderSettings.skybox = level.SkyboxMaterial;
            }
        }

        private void ClearChilds()
        {
            List<GameObject> childToDestroy = new List<GameObject>();
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject destroyObject = transform.GetChild(i).gameObject;
                childToDestroy.Add(destroyObject);
            }

            foreach (GameObject child in childToDestroy)
            {
                DestroyImmediate(child);
            }
        }


        #region Events

        public void SendStart()
        {
            OnLevelStarted?.Invoke();
            string content = (PlayerPrefs.GetInt(LEVEL_COUNT, 0) + 1).ToString();
        }

        public void SendRestart()
        {
            OnLevelRestarted?.Invoke();
            string content = (PlayerPrefs.GetInt(LEVEL_COUNT, 0) + 1).ToString();
        }

        public void SendComplete()
        {
            OnLevelCompleted?.Invoke();
            string content = (PlayerPrefs.GetInt(LEVEL_COUNT, 0) + 1).ToString();
        }

        #endregion
    }

    [System.Serializable]
    public class Level
    {
        public GameObject LevelPrefab;
        public Material SkyboxMaterial;
    }
}