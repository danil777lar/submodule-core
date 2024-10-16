using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private SoundType soundType;
    
    [InjectService] private SoundService _soundService;
    
    private void Start()
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);
    }
    
    [ContextMenu("Play Sound")]
    public void PlaySound()
    {
        _soundService.Play(soundType)
            .SetTarget(this);
    }

    [ContextMenu("Stop Sound")]
    public void StopSound()
    {
        this.SoundServiceStop();
    }
}
