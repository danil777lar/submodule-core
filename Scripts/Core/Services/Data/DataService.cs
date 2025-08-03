using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Larje.Core.Services
{ 
    [BindService(typeof(IDataService))]
    public class DataService : Service, IDataService
    {
        private const string DATA_PATH = "Saves"; 
        private const string SAVE_FILE_EXTENSION = ".save"; 
        private const string META_FILE_EXTENSION = ".meta"; 
        
        [SerializeField] private bool loadGameDataOnInit = true;
        [Space]
        [SerializeField] private SystemData systemData;
        [SerializeField] private GameData gameData;

        private string _systemSaveName = "system";
        private string _gameSaveName = "default";
        
        public SystemData SystemData => systemData;
        public GameData GameData => gameData;
        
        private string SystemSavePath => Path.Combine(Application.persistentDataPath, _systemSaveName);

        public override void Init()
        {
            InitSystemData();
            InitGameData();
        }
        
        public void SaveGameData(string saveName = "")
        {
            if (!string.IsNullOrEmpty(saveName))
            {
                _gameSaveName = saveName;
            }
            
            SaveMetaData metaData = new SaveMetaData
            {
                name = _gameSaveName,
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                savePath = GetSavePath(_gameSaveName + SAVE_FILE_EXTENSION),
                imagePath = string.Empty
            };
            
            WriteFile(GetSavePath(_gameSaveName + META_FILE_EXTENSION), metaData);
            WriteFile(GetSavePath(_gameSaveName + SAVE_FILE_EXTENSION), gameData);
        }

        public bool LoadGameData(string saveName = "")
        {
            if (TryReadFile(GetSavePath(_gameSaveName + SAVE_FILE_EXTENSION), out GameData result))
            {
                if (!string.IsNullOrEmpty(saveName))
                {
                    _gameSaveName = saveName;
                }
                gameData = result;
                return true;
            }

            return false;
        }
        
        public void SaveSystemData()
        {
            WriteFile(SystemSavePath, systemData);
        }
        
        [ContextMenu("Clear Progress")]
        public void DeleteAllData()
        {
            try
            {
                string savePath = GetSavePath();
                if (Directory.Exists(savePath))
                {
                    Directory.Delete(savePath, true);
                }
                File.Delete(SystemSavePath);
                
                InitSystemData();
                InitGameData();
            }
            catch (Exception e)
            {
                Debug.LogError($"DataService: Failed to delete all data: {e.Message}");
            }
        }

        public List<SaveMetaData> GetSaves()
        {
            List<SaveMetaData> saves = new List<SaveMetaData>();
            CheckExistDirectory(GetSavePath());
            string[] files = Directory.GetFiles(GetSavePath(), "*" + META_FILE_EXTENSION, SearchOption.AllDirectories);
            foreach (string file in files)
            {
                if (TryReadFile(file, out SaveMetaData metaData))
                {
                    saves.Add(metaData);
                }
            }
            return saves;
        }

        private void OnDestroy()
        {
            SaveSystemData();
        }

        private void InitSystemData()
        {
            if (!TryReadFile(SystemSavePath, out this.systemData))
            {
                this.systemData = new SystemData();
                WriteFile(SystemSavePath, systemData);
            }
            
            systemData.IternalData.SessionNum++;
            SaveSystemData();
        }

        private void InitGameData()
        {
            if (loadGameDataOnInit && !LoadGameData())
            {
                gameData = new GameData();
                SaveGameData();
            }
        }

        private bool TryReadFile<T>(string path, out T result)
        {
            result = default;
            CheckExistDirectory(Path.GetDirectoryName(path));
            if (!File.Exists(path))
            {
                return false;
            }
            
            try
            {
                string json = File.ReadAllText(path);
                result = JsonUtility.FromJson<T>(json);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"DataService: Failed to read file {path}: {e.Message}");
                return false;
            }
        }
        
        private void WriteFile(string path, object data)
        {
            CheckExistDirectory(path);
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
        }

        private void CheckExistDirectory(string path)
        {
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
        
        [ContextMenu("Open Data Path")]
        private void OpenDataPath()
        {
            CheckExistDirectory(GetSavePath());
            Debug.Log(GetSavePath());
            LarjeSystemUtility.OpenFileExplorer(GetSavePath());
        }

        private string GetSavePath(string fileName = "")
        {
            return Path.Combine(Application.persistentDataPath, DATA_PATH, fileName);
        }
    }
}