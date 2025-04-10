﻿using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services
{ 
    [BindService(typeof(DataService))]
    public class DataService : Service
    {
        [SerializeField] protected string _profileName;
        [SerializeField] protected DefaultProfile _defaultProfile;
        [SerializeField] protected GameData _data;
        private string FilePath => Path.Combine(Application.persistentDataPath, _profileName + ".json");

        public GameData Data => _data;

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
            byte[] jsonDataBytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(_data, false));
            File.WriteAllText(FilePath, Convert.ToBase64String(jsonDataBytes));
        }
        
        [ContextMenu("Open Data Path")]
        public virtual void OpenDataPath()
        {
            string path = Path.GetDirectoryName(FilePath);
            System.Diagnostics.Process.Start("explorer.exe","/select,"+path);
        }
        
        [ContextMenu("Clear Progress")]
        public virtual void DeleteSave()
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }

        protected virtual void Load()
        {
            if (File.Exists(FilePath))
            {
                _data = JsonUtility.FromJson<GameData>(Encoding.UTF8.GetString(Convert.FromBase64String(File.ReadAllText(FilePath))));
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
    }
}