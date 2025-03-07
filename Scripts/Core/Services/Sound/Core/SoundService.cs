using System;
using System.Collections.Generic;
using ProjectConstants;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace Larje.Core.Services
{
    [BindService(typeof(SoundService))]
    public class SoundService : Service
    {
        [SerializeField] private SoundServiceConfig config;
        
        [InjectService] private DataService _dataService;
        
        private bool _isLoaded;
        private List<SoundData> _sounds = new List<SoundData>();
        private List<Action> _onLoaded = new List<Action>();
        private UnityEvent<float> OnUpdate = new UnityEvent<float>();

        public override void Init()
        {
            config.LoadSounds(() =>
            {
                _isLoaded = true;
                foreach (Action onLoaded in _onLoaded)
                {
                    onLoaded?.Invoke();
                }
                _onLoaded.Clear();
            });
        }

        public Sound Play(SoundType soundType)
        {
            AssetReference soundReference = config.GetSound(soundType);
            if (soundReference == null)
            {
                Debug.LogError($"Sound Service: Sound {soundType} not found");
                return null;
            }

            SoundData data = new SoundData();
            _sounds.Add(data);

            data.Sound = new Sound(soundReference.InstantiateAsync(transform), OnUpdate, data.OnDestroy, GetChannel)
                .AddDestroy(() => Stop(data.Sound));
            
            return data.Sound;
        }
        
        public void Stop(Sound sound)
        {
            Stop(_sounds.Find(x => x.Sound == sound));
        }
        
        public void Stop(object target)
        {
            _sounds.FindAll(x => x.Sound.Target == target).ForEach(Stop);
        }
        
        public void RunWhenSoundsLoaded(Action onLoaded)
        {
            if (_isLoaded)
            {
                onLoaded?.Invoke();
            }
            else
            {
                _onLoaded.Add(onLoaded);
            }
        }
        
        private void Stop(SoundData data)
        {
            _sounds.Remove(data);
            data?.OnDestroy?.Invoke();
        }

        private void Update()
        {
            OnUpdate?.Invoke(Time.deltaTime);
        }
        
        private SoundChannelData GetChannel(string channelName)
        {
            return _dataService.Data.Settings.SoundData.GetChannel(channelName);
        }
        
        private class SoundData
        {
            public Sound Sound;
            public UnityEvent OnDestroy = new UnityEvent();
        }
    }
}