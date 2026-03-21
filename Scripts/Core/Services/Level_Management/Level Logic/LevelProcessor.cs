using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProjectConstants;
using UnityEngine;

namespace Larje.Core.Services
{
    public abstract class LevelProcessor : MonoBehaviour
    {
        [InjectService] private IGameStateService _gameStateService;

        public bool IsLevelPlaying => _gameStateService.CurrentState == GameStates.Playing;

        public event Action<StartData> EventLevelStart;
        public event Action<StopData> EventLevelStop;

        public abstract void TryStartLevel(StartData data);

        public abstract void TryStopLevel(StopData data);

        public abstract LevelData GetLevelData();

        protected virtual void Awake()
        {
            DIContainer.InjectTo(this, typeof(LevelProcessor));
        }

        protected void StartLevel(StartData data)
        {
            if (!IsLevelPlaying)
            {
                GetComponentsInChildren<ILevelStartHandler>(true)
                    .ToList().ForEach(x => x.OnLevelStarted(data));
                EventLevelStart?.Invoke(data);
            }
        }

        protected void StopLevel(StopData data)
        {
            if (IsLevelPlaying)
            {
                GetComponentsInChildren<ILevelEndHandler>(true)
                    .ToList().ForEach(x => x.OnLevelEnded(data));
                EventLevelStop?.Invoke(data);
            }
        }

        public class LevelData { }

        public class StartData
        {
            public readonly LevelStartType StartType;

            public StartData(LevelStartType startType)
            {
                StartType = startType;
            }
        }

        public class StopData
        {
            public readonly bool IsWin;
            public readonly LevelStopType StopType;

            public StopData(bool isWin, LevelStopType stopType)
            {
                IsWin = isWin;
                StopType = stopType;
            }
        }
    }
}
