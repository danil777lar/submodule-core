using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Larje.Core.Services
{
    public abstract class LevelProcessor : MonoBehaviour
    {
        private bool _isLevelPlaying = false;

        public abstract void TryStartLevel(StartData data);

        public abstract void TryStopLevel(StopData data);

        public abstract float GetLevelProgress();

        protected void StartLevel(StartData data)
        {
            if (!_isLevelPlaying)
            {
                GetComponentsInChildren<ILevelStartHandler>(true)
                    .ToList().ForEach(x => x.OnLevelStarted(data));
                _isLevelPlaying = true;
            }
        }

        protected void StopLevel(StopData data)
        {
            if (_isLevelPlaying)
            {
                GetComponentsInChildren<ILevelEndHandler>(true)
                    .ToList().ForEach(x => x.OnLevelEnded(data));
                _isLevelPlaying = false;
            }
        }

        public abstract class StartData { }

        public abstract class StopData { }
    }
}