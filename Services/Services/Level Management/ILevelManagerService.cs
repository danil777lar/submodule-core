using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;

namespace Larje.Core.Services
{
    public interface ILevelManagerService
    {
        public bool IsInstantiatingLevel();
        
        public int GetCurrentLevelCount();

        public int GetCurrentLevelIndex();

        public LevelProcessor.LevelData GetCurrentLevelData();

        public void SpawnCurrentLevel();
        
        public void IncrementLevelId();
        
        public void TryStartCurrentLevel(LevelProcessor.StartData data);

        public void TryStopCurrentLevel(LevelProcessor.StopData data);
    }
}