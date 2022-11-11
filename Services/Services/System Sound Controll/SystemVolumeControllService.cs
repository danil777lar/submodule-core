using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services.NativeVolume
{
    [BindService(typeof(SystemVolumeControllService))]
    public class SystemVolumeControllService : Service
    {
        private float _lastVolumeValue;
        private INativeVolumeService _nativeService;

        public Action<float> VolumeChanged;


        private void Update()
        {
            if (_nativeService != null) 
            {
                float currentSystemVolume = GetSystemVolume();
                if (_lastVolumeValue != currentSystemVolume) 
                {
                    VolumeChanged?.Invoke(currentSystemVolume);
                }
                _lastVolumeValue = currentSystemVolume;
            }
        }

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

            if (_nativeService != null)
            {
                _lastVolumeValue = GetSystemVolume();
            }
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