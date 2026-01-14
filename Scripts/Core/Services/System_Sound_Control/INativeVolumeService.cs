using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services.NativeVolume
{
    public interface INativeVolumeService
    {
        public float GetSystemVolume();

        public void SetSystemVolume(float volumeValue);
    }
}