using UnityEngine;

namespace Larje.Core.Tools
{
    public class ProjectInitialization : MonoBehaviour
    {
        [SerializeField] private int targetFrameRate;
        [SerializeField] private bool logEnabled;
        
        private int _framesCount;
        private int _lastFramesCount;
        private float _time;
        
        private void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.targetFrameRate = targetFrameRate;
            Debug.unityLogger.logEnabled = logEnabled;
        }
    }
}