using UnityEngine;
using MoreMountains.Tools;

namespace Larje.Core.Tools
{
    public class ProjectInitialization : MonoBehaviour
    {
        [SerializeField] private int targetFrameRate;
        [SerializeField] private bool drawFrameRate;
        [SerializeField] private bool logEnabled;
        [SerializeField] private bool logSystemInfo;
        
        private int _framesCount;
        private int _lastFramesCount;
        private float _time;
        
        private void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.targetFrameRate = targetFrameRate;
            Debug.unityLogger.logEnabled = logEnabled;
            if (logSystemInfo)
            {
                Debug.Log(MMDebug.GetSystemInfo());
            }
        }

        private void Update()
        {
            if (drawFrameRate)
            {
                _time += Time.deltaTime;
                _framesCount++;
                
                if (_time >= 1f)
                {
                    _lastFramesCount = _framesCount;
                    _framesCount = 0;
                    _time = 0;
                }
                
                MMDebug.DebugOnScreen($"FPS: {_lastFramesCount}");
            }
        }
    }
}