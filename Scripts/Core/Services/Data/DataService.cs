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
        private const int META_DATA_LINE = 1;
        private const int CONTENT_DATA_LINE = 2;
        
        private const string DATA_PATH = "Saves"; 
        private const string SAVE_FILE_EXTENSION = ".save"; 
        
        [SerializeField] private bool loadGameDataOnInit = true;
        [Space]
        [SerializeField] private SystemData systemData;
        [SerializeField] private GameData gameData;

        private string _systemSaveName = "system";
        private string _gameSaveName = "default";
        
        private List<IMetaDataWriter> _metaDataWriters = new List<IMetaDataWriter>();
        
        public SystemData SystemData => systemData;
        public GameData GameData => gameData;
        
        public event Action EventPreSave;
        public event Action EventAfterLoad;
        
        private string SystemSavePath => Path.Combine(Application.persistentDataPath, _systemSaveName);

        public override void Init()
        {
            _metaDataWriters = GetComponents<IMetaDataWriter>().ToList();
            
            InitSystemData();
            InitGameData();
        }

        public void SetClearData(string saveName = "")
        {
            if (!string.IsNullOrEmpty(saveName))
            {
                _gameSaveName = saveName;
            }

            gameData = new GameData();
            string serialized = JsonUtility.ToJson(GetMetaData(), false);
            gameData = JsonUtility.FromJson<GameData>(serialized);

            EventAfterLoad?.Invoke();
        }
        
        public void SaveGameData(string saveName = "")
        {
            if (!string.IsNullOrEmpty(saveName))
            {
                _gameSaveName = saveName;
            }
            
            EventPreSave?.Invoke();
            WriteFile(GetSavePath(_gameSaveName + SAVE_FILE_EXTENSION), gameData, true);
        }

        public bool LoadGameData(string saveName = "")
        {
            string save = string.IsNullOrEmpty(saveName) ? _gameSaveName : saveName;
            string path = GetSavePath(save + SAVE_FILE_EXTENSION);
            if (TryReadFile(path, CONTENT_DATA_LINE, out GameData result))
            {
                _gameSaveName = save;
                gameData = result;
                EventAfterLoad?.Invoke();
                return true;
            }

            return false;
        }
        
        public void SaveSystemData()
        {
            WriteFile(SystemSavePath, systemData, false);
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
                Debug.LogWarning($"DataService: Failed to delete all data: {e.Message}");
            }
        }

        public List<SaveMetaData> GetSaves()
        {
            List<SaveMetaData> saves = new List<SaveMetaData>();
            CheckExistDirectory(GetSavePath(), false);
            string[] files = Directory.GetFiles(GetSavePath(), "*" + SAVE_FILE_EXTENSION, SearchOption.AllDirectories);
            foreach (string file in files)
            {
                if (TryReadFile(file, META_DATA_LINE, out SaveMetaData metaData))
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
            if (!TryReadFile(SystemSavePath, CONTENT_DATA_LINE, out this.systemData))
            {
                this.systemData = new SystemData();
                WriteFile(SystemSavePath, systemData, false);
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
                LoadGameData();
            }
        }

        private bool TryReadFile<T>(string path, int contentLine, out T result)
        {
            result = default;
            try
            {
                string json = ReadFileLine(path, contentLine);
                if (!string.IsNullOrEmpty(json))
                {
                    result = JsonUtility.FromJson<T>(json);
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"DataService: Failed to read file {path}: {e.Message}");
            }
            
            return false;
        }

        private string ReadFileLine(string path, int line)
        {
            try
            {
                return File.ReadLines(path).Skip(line - 1).FirstOrDefault();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"DataService: Failed to read line {line} from file {path}: {e.Message}");
                return "";
            }
        }
        
        private void WriteFile(string path, object data, bool writeMetaData)
        {
            CheckExistDirectory(path, true);
            
            string content = string.Empty;
            if (writeMetaData)
            {
                content += JsonUtility.ToJson(GetMetaData(), false) + Environment.NewLine;
            }
            else
            {
                content += Environment.NewLine;
            }
            content += JsonUtility.ToJson(data, false);
            File.WriteAllText(path, content);
        }

        private void CheckExistDirectory(string path, bool removeFileName)
        {
            string dir = path;
            if (removeFileName)
            {
                dir = Path.GetDirectoryName(path);
            }
            
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
        
        [ContextMenu("Open Data Path")]
        private void OpenDataPath()
        {
            CheckExistDirectory(GetSavePath(), false);
            Debug.Log(GetSavePath());
            LarjeSystemUtility.OpenFileExplorer(GetSavePath());
        }

        private string GetSavePath(string fileName = "")
        {
            return Path.Combine(Application.persistentDataPath, DATA_PATH, fileName);
        }

        private SaveMetaData GetMetaData()
        {
            SaveMetaData metaData = new SaveMetaData
            {
                name = _gameSaveName,
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            foreach (IMetaDataWriter metaWriter in _metaDataWriters)
            {
                metaWriter.WriteMetaData(metaData);
            }
            
            return metaData;
        }
    }
}
