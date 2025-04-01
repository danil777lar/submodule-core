using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProjectConstants;
using UnityEngine;

namespace Larje.Core.Services
{
    public abstract class LevelProcessor : MonoBehaviour
    {
        protected bool isLevelPlaying = false;
        
        public bool IsLevelPlaying => isLevelPlaying;

        public abstract void TryStartLevel(StartData data);

        public abstract void TryStopLevel(StopData data);

        public abstract LevelData GetLevelData();

        public void SendEvent(LevelEvent levelEvent)
        {
            GetComponentsInChildren<ILevelEventHandler>(true)
                .ToList().ForEach(x => x.OnLevelEvent(levelEvent));
        }
        
        protected void StartLevel(StartData data)
        {
            if (!isLevelPlaying)
            {
                GetComponentsInChildren<ILevelStartHandler>(true)
                    .ToList().ForEach(x => x.OnLevelStarted(data));
                isLevelPlaying = true;
            }
        }

        protected void StopLevel(StopData data)
        {
            if (isLevelPlaying)
            {
                GetComponentsInChildren<ILevelEndHandler>(true)
                    .ToList().ForEach(x => x.OnLevelEnded(data));
                isLevelPlaying = false;
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
            public readonly LevelStopType StopType;

            public StopData(LevelStopType stopType)
            {
                StopType = stopType;
            }
        }
    }
}