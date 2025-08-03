using System;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services
{
    public interface IDataService
    {
        public SystemData SystemData { get; }
        public GameData GameData { get; }
        
        public void Save();
        public void Save(string saveName);
        public void DeleteSave();
        public List<string> GetSaves();
    }
}
