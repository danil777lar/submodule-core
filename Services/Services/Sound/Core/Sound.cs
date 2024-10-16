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
    private Func<float> _volume = () => 1f;
    private Func<float> _pitch = () => 1f;
    private Func<float> _spatialBlend = () => 0f;
    private Func<Vector3> _position = () => Vector3.zero;
    
    private List<Action> _onComplete = new List<Action>();
    private List<Action> _onDestroy = new List<Action>();
 
    private object _target;
    
    private GameObject _soundObject;
    private AudioSource _audioSource;
    
    private UnityEvent<float> _eventUpdate;
    private UnityEvent _eventDestroy;
    
    public object Target => _target;
    
    public Sound(AsyncOperationHandle<GameObject> operation, UnityEvent<float> eventUpdate, UnityEvent eventDestroy)
    {
        operation.Completed += OnSoundLoaded;
        
        _eventUpdate = eventUpdate;
        _eventDestroy = eventDestroy;
        
        Subscribe();
    }
    
    public Sound SetLoop(int loops)
    {
        _loops = loops;
        return this;
    }

    public Sound SetLoop(bool loop)
    {
        _loops = loop ? -1 : 0;
        return this;
    }

    public Sound SetVolume(Func<float> volume)
    {
        _volume = volume;
        return this;
    }

    public Sound SetPitch(Func<float> pitch)
    {
        _pitch = pitch;
        return this;
    }
    
    public Sound SetSpatialBlend(Func<float> spatialBlend)
    {
        _spatialBlend = spatialBlend;
        return this;
    }

    public Sound SetPosition(Func<Vector3> position)
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
