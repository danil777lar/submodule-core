using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Larje.Core.Services.LevelManagement
{
    public abstract class LevelProcessor : MonoBehaviour
    {
        private List<ILevelStartHandler> _startHandlers;
        private List<ILevelEndHandler> _endHandlers;

        private bool _isLevelPlaying = false;


        protected virtual void Start()
        {
            _startHandlers = GetComponentsInChildren<ILevelStartHandler>(true).ToList();
            _startHandlers.AddRange(GetComponents<ILevelStartHandler>());
            _endHandlers = GetComponentsInChildren<ILevelEndHandler>(true).ToList();
            _endHandlers.AddRange(GetComponents<ILevelEndHandler>());
        }

        public abstract void TryStartLevel(StartData data);

        public abstract void TryStopLevel(StopData data);

        protected void StartLevel(StartData data)
        {
            if (!_isLevelPlaying)
            {
                _startHandlers.ForEach(x => x.OnLevelStarted(data));
                _isLevelPlaying = true;
            }
        }

        protected void StopLevel(StopData data)
        {
            if (_isLevelPlaying)
            {
                _endHandlers.ForEach(x => x.OnLevelEnded(data));
                _isLevelPlaying = false;
            }
        }


        public abstract class StartData { }

        public abstract class StopData { }
    }
}