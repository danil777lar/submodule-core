using UnityEngine;
using MoreMountains.Tools;

namespace Larje.Core.Tools
{
    public class ProjectInitialization : MonoBehaviour
    {
        [SerializeField] private int targetFrameRate;
        [SerializeField] private bool logEnabled;
        [SerializeField] private bool logSystemInfo;
        
        private void Start()
        {
            Application.targetFrameRate = targetFrameRate;
            Debug.unityLogger.logEnabled = logEnabled;
            if (logSystemInfo)
            {
                Debug.Log(MMDebug.GetSystemInfo());
            }
        }
    }
}