using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceClipRandomizer : MonoBehaviour, IAudioSourceOnNewLoopHandler
    {
        [SerializeField] private AudioClip[] clips;

        private AudioSource _audioSource;
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            SetClip();
        }
        
        public void OnNewLoop()
        {
            SetClip();
        }

        private void SetClip()
        {
            _audioSource.clip = clips[Random.Range(0, clips.Length)];
        }
    }
}