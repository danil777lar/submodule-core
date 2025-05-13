using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core;
using Larje.Core.Services;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private SoundSettings sound;
    [Space] 
    [SerializeField] private bool playOnStart;
    [SerializeField] private int loops = 1;
    [SerializeField, Range(0f, 1f)] private float volume = 1f;
    [SerializeField, Range(0f, 2f)] private float pitch = 1f;
    [SerializeField, Range(0f, 1f)] private float spatialBlend = 0f;
    
    [InjectService] private SoundService _soundService;
    
    private void Start()
    {
        DIContainer.InjectTo(this);
        if (playOnStart)
        {
            PlaySound();
        }
    }

    private void OnDestroy()
    {
        StopSound();
    }

    private void OnDisable()
    {
        StopSound();
    }

    [ContextMenu("Play Sound")]
    public void PlaySound()
    {
        sound.Play(x => x
            .SetTarget(this)
            .SetLoop(loops)
            .SetVolume(t => volume)
            .SetPosition(t => transform.position)
            .SetPitch(t => pitch)
            .SetSpatialBlend(t => spatialBlend));
    }

    [ContextMenu("Stop Sound")]
    public void StopSound()
    {
        this.SoundServiceStop();
    }
}
