using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Larje.Core.Services
{ 
    [BindService(typeof(DataService))]
    public class DataService : Service
    {
        private const string DATA_PATH = "Saves"; 
        private const string FILE_EXTENSION = ".save"; 
        
        [SerializeField] protected DefaultProfile _defaultProfile;
        [SerializeField] protected GameData _data;

        private string _saveName = "default";
        
        public GameData Data => _data;
        
        private string SaveDir => Path.Combine(Application.persistentDataPath, DATA_PATH);
        private string SavePath => Path.Combine(SaveDir, _saveName);

        public override void Init()
        {
            Load();
            _data.IternalData.SessionNum++;
            Save();
        }
        
        public virtual void SetDefaultData()
        {
            if (_defaultProfile == null) 
            {
                Debug.LogError($"Data Service: Default Data Profile is null", gameObject);
                return;
            }

            _data = _defaultProfile.profileData;
            Save();
            Load();
        }

        [ContextMenu("Save")]
        public virtual void Save()
        {
            Save("");
        }
        
        public virtual void Save(string saveName)
        {
            if (!string.IsNullOrEmpty(saveName))
            {
                _saveName = saveName;
            }
            
            byte[] jsonDataBytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(_data, false));
            CheckExistDirectory(SaveDir);
            File.WriteAllText(SavePath, Convert.ToBase64String(jsonDataBytes));
        }
        
        [ContextMenu("Open Data Path")]
        public virtual void OpenDataPath()
        {
            CheckExistDirectory(SaveDir);
            System.Diagnostics.Process.Start("explorer.exe","/select,"+SaveDir);
        }
        
        [ContextMenu("Clear Progress")]
        public virtual void DeleteSave()
        {
            CheckExistDirectory(SaveDir);
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
            }
        }

        public List<string> GetSaves()
        {
            CheckExistDirectory(SaveDir);
            string[] files = Directory.GetFiles(SaveDir, "*" + FILE_EXTENSION, SearchOption.AllDirectories);
            return files.ToList();
        }

        protected virtual void Load()
        {
            CheckExistDirectory(SaveDir);
            if (File.Exists(SavePath))
            {
                _data = JsonUtility.FromJson<GameData>(Encoding.UTF8.GetString(Convert.FromBase64String(File.ReadAllText(SavePath))));
            }
            else
            {
                SetDefaultData();
            }
        }

        protected virtual void OnDestroy()
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
    }
}