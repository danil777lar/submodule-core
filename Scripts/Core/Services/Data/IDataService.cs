using System;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services
{
    public interface IDataService
    {
        public SystemData SystemData { get; }
        public GameData GameData { get; }
        
        public void SetClearData(string saveName = "");
        public void SaveGameData(string saveName = "");
        public bool LoadGameData(string saveName = "");

        public List<SaveMetaData> GetSaves();

        public void SaveSystemData();

        public void DeleteAllData();
        
        public event Action EventPreSave;
        public event Action EventAfterLoad;
    }
}
