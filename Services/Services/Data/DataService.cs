using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services.DataService
{
    [BindService(typeof(DataService))]
    public class DataService : Service
    {
        [SerializeField] private string _profileName;
        [SerializeField] private DefaultProfile _defaultProfile;
        [SerializeField] private GameData _data;
        private string FilePath => Path.Combine(Application.persistentDataPath, _profileName + ".json");

        public GameData Data => _data;


        public override void Init()
        {
            Load();
            _data.IternalData.SessionNum++;
            Save();
        }

        private void Load()
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

        private void OnDestroy()
        {
            Save();
        }

        [ContextMenu("Save")]
        public void Save()
        {
            byte[] jsonDataBytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(_data, false));
            File.WriteAllText(FilePath, Convert.ToBase64String(jsonDataBytes));
        }

        public void SetDefaultData()
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


        #region Debug
        [ContextMenu("Clear Progress")]
        public void DeleteSave()
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }
        #endregion
    }
}