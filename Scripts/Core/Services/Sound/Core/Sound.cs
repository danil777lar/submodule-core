using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

public class Sound
{
    private int _loops = 1;
    private float _loopLength = 0f;
    private float _defaultVolume = 1f;
    private float _defaultPitch = 1f;
    private string _channel = "Default";
    
    private Func<float, float> _volume = (t) => 1f;
    private Func<float, float> _pitch = (t) => 1f;
    private Func<float, float> _spatialBlend = (t) => 0f;
    private Func<float, Vector3> _position = (t) => Vector3.zero;
    private Func<string, SoundChannelData> _getChannel;
    
    private List<Action> _onComplete = new List<Action>();
    private List<Action> _onDestroy = new List<Action>();
 
    private object _target;
    
    private GameObject _soundObject;
    private AudioSource _audioSource;
    
    private UnityEvent<float> _eventUpdate;
    private UnityEvent _eventDestroy;
    
    public object Target => _target;
    
    public Sound(AsyncOperationHandle<GameObject> operation, UnityEvent<float> eventUpdate, UnityEvent eventDestroy, 
        Func<string, SoundChannelData> getChannel)
    {
        operation.Completed += OnSoundLoaded;
        
        _eventUpdate = eventUpdate;
        _eventDestroy = eventDestroy;
        _getChannel = getChannel;
        
        Subscribe();
    }
    
    public Sound SetLoop(int loops)
    {
        _loops = loops;
        return this;
    }

    public Sound SetLoop(bool loop)
    {
        _loops = loop ? -1 : 1;
        return this;
    }
    
    public Sound SetChannel(string channel)
    {
        _channel = channel;
        return this;
    }

    public Sound SetVolume(Func<float, float> volume)
    {
        _volume = volume;
        return this;
    }

    public Sound SetPitch(Func<float, float> pitch)
    {
        _pitch = pitch;
        return this;
    }
    
    public Sound SetSpatialBlend(Func<float, float> spatialBlend)
    {
        _spatialBlend = spatialBlend;
        return this;
    }

    public Sound SetPosition(Func<float, Vector3> position)
    {
        _position = position;
        return this;
    }

    public Sound SetTarget(object target)
    {
        _target = target;
        return this;
    }

    public Sound AddComplete(Action onComplete)
    {
        _onComplete.Add(onComplete);
        return this;
    }
    
    public Sound AddDestroy(Action onDestroy)
    {
        _onDestroy.Add(onDestroy);
        return this;
    }

    private void OnSoundLoaded(AsyncOperationHandle<GameObject> operation)
    {
        _soundObject = operation.Result;
        _audioSource = _soundObject.GetComponent<AudioSource>();

        _audioSource.loop = true;
        
        _defaultVolume = _audioSource.volume;
        _defaultPitch = _audioSource.pitch;
    }
    
    private void Subscribe()
    {
        _eventUpdate.AddListener(Update);
        _eventDestroy.AddListener(Destroy);
    }
    
    private void Unsubscribe()
    {
        _eventUpdate.RemoveListener(Update);
        _eventDestroy.RemoveListener(Destroy);
    }

    private void Update(float deltaTime)
    {
        if (_soundObject == null || _audioSource == null) return;

        float t = _loopLength / _audioSource.clip.length;
        
        _audioSource.volume = _volume(t) * _defaultVolume * _getChannel.Invoke(_channel).Volume;
        _audioSource.pitch = _pitch(t) * _defaultPitch;
        _audioSource.spatialBlend = _spatialBlend(t);
        _audioSource.transform.position = _position(t);
        
        _loopLength += deltaTime * _audioSource.pitch;
        if (_loopLength >= _audioSource.clip.length)
        {
            LoopComplete();
        }
    }

    private void LoopComplete()
    {
        _loopLength = 0f;
        if (_loops < 0) return;
        
        _loops--;
        if (_loops <= 0)
        {
            Complete();
            Destroy();
        }
    }

    private void Complete()
    {
        _onComplete.ForEach(x => x.Invoke());
    }

    private void Destroy()
    {
        Object.Destroy(_soundObject);
        Unsubscribe();
        _onDestroy.ForEach(x => x.Invoke());
    }
}
