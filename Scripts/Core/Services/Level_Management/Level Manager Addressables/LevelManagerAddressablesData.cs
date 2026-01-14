using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Larje.Core.Services
{
    public partial class GameData
    {
        public LevelManagerAddressablesData levelManagerAddressablesData;
    }

    [Serializable]
    public class LevelManagerAddressablesData
    {
        [SerializeField] public int CurrentLevelCount;
        [SerializeField] public int LastLevelIndex;
        [SerializeField] public List<int> RandomLevels;
    }
}
