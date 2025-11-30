using System;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services
{
    public partial class GameData
    {
        public LevelObjectsDatabase LevelObjectsDatabase;
    }
}

[Serializable]
public class LevelObjectsDatabase
{
    public List<LevelObjectData> Objects = new List<LevelObjectData>();
}

[Serializable]
public class LevelObjectData
{
    public string guid;
    public List<LevelObjectDataWrapper> dataList = new List<LevelObjectDataWrapper>();
} 

[Serializable]
public class LevelObjectDataWrapper
{
    public string type;
    public string json;
}