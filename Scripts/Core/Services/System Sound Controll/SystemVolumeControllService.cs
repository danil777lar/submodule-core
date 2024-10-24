using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services.NativeVolume
{
    [BindService(typeof(SystemVolumeControllService))]
    public class SystemVolumeControllService : Service
    {
        private INativeVolumeService _nativeService;


        public override void Init()
        {
#if UNITY_EDITOR
            _nativeService = null;
#elif UNITY_ANDROID
            _nativeService = new AndroidNativeVolumeService();
#elif UNITY_IOS
            _nativeService = null;
#else
            Debug.LogError("Unexpected platform");
#endif
        }

        public float GetSystemVolume() 
        {
            return _nativeService != null ? _nativeService.GetSystemVolume() : 0f;
        }

        public void SetSystemVolume(float volume) 
        {
            if (_nativeService != null)
            {
                _nativeService.SetSystemVolume(volume);
            }
        }
    }
}