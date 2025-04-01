using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core;
using Larje.Core.Services;
using UnityEngine;

[Serializable]
public class SoundSettings
{
    [field: SerializeField] public bool Use;
    [field: SerializeField] public SoundType SoundType;

    public void Play(Action<Sound> onLoaded = null)
    {
        if (Use)
        {
            SoundService soundService = DIContainer.GetService<SoundService>();
            soundService.RunWhenSoundsLoaded(() =>
            {
                Sound sound = soundService.Play(SoundType);
                onLoaded?.Invoke(sound);    
            });
        }
    }
}
