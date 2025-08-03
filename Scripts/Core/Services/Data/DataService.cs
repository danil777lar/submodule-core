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
        private const string FILE_EXTENSION = ".save"; 
        
        [SerializeField] protected SystemData systemData;
        [SerializeField] protected GameData gameData;

        private string _systemSaveName = "system";
        private string _gameSaveName = "default";
        
        public SystemData SystemData => systemData;
        public GameData GameData => gameData;
        
        private string GameSaveDir => Path.Combine(Application.persistentDataPath, DATA_PATH);
        private string GameSavePath => Path.Combine(GameSaveDir, _gameSaveName + FILE_EXTENSION);
        private string SystemSavePath => Path.Combine(Application.persistentDataPath, _systemSaveName + FILE_EXTENSION);

        public override void Init()
        {
            Load();
            systemData.IternalData.SessionNum++;
            Save();
        }

        [ContextMenu("Save")]
        public void Save()
        {
            Save("");
        }
        
        public void Save(string saveName)
        {
            if (!string.IsNullOrEmpty(saveName))
            {
                _gameSaveName = saveName;
            }
            
            byte[] jsonDataBytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(gameData, false));
            CheckExistDirectory(GameSaveDir);
            File.WriteAllText(GameSavePath, Convert.ToBase64String(jsonDataBytes));
        }
        
        [ContextMenu("Clear Progress")]
        public void DeleteSave()
        {
            CheckExistDirectory(GameSaveDir);
            if (File.Exists(GameSavePath))
            {
                File.Delete(GameSavePath);
            }
        }

        public List<string> GetSaves()
        {
            CheckExistDirectory(GameSaveDir);
            string[] files = Directory.GetFiles(GameSaveDir, "*" + FILE_EXTENSION, SearchOption.AllDirectories);
            return files.ToList();
        }

        private void Load()
        {
            CheckExistDirectory(GameSaveDir);
            if (File.Exists(GameSavePath))
            {
                gameData = JsonUtility.FromJson<GameData>(Encoding.UTF8.GetString(Convert.FromBase64String(File.ReadAllText(GameSavePath))));
            }
            else
            {
                SetDefaultData();
            }
        }

        private void OnDestroy()
        {
            Save();
        }

        private void CheckExistDirectory(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
        
        [ContextMenu("Open Data Path")]
        private void OpenDataPath()
        {
            CheckExistDirectory(GameSaveDir);
            
            Debug.Log(GameSaveDir);
            LarjeSystemUtility.OpenFileExplorer(GameSaveDir);
        }
        
        private void SetDefaultData()
        {
            gameData = new GameData();
            Save();
            Load();
        }
    }
}