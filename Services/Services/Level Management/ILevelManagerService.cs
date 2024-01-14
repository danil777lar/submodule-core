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

        public T GetCurrentLevelData<T>() where T : LevelProcessor.LevelData;

        public void SpawnCurrentLevel();
        
        public void IncrementLevelId();
        
        public void TryStartCurrentLevel(LevelProcessor.StartData data);

        public void TryStopCurrentLevel(LevelProcessor.StopData data);
    }
}