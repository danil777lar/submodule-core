using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;

namespace Larje.Core.Services
{
    public interface ILevelManagerService
    {
        public event Action<LevelProcessor> EventLevelInstantiated;
        
        public bool IsLevelPlaying { get; }

        public bool IsInstantiatingLevel();
        
        public int GetCurrentLevelCount();

        public int GetCurrentLevelIndex();
        public void SetCurrentLevelIndex(int id);

        public T GetCurrentLevelData<T>() where T : LevelProcessor.LevelData;

        public void SpawnCurrentLevel();
        
        public void IncrementLevelId();

        public void TryStartCurrentLevel(LevelProcessor.StartData data);

        public void TryStopCurrentLevel(LevelProcessor.StopData data);
    }
}
