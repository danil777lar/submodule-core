using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CinemachineFovListener : MonoBehaviour
{
    private static Dictionary<int, List<CinemachineFovListener>> _listeners;
 
    public static void SetFovModifier(int channel, float modifier)
    {
        foreach (var listener in GetListenersList(channel))
        {
            listener.SetModifier(modifier);
        }
    }
    
    private static List<CinemachineFovListener> GetListenersList(int channel)
    {
        _listeners ??= new Dictionary<int, List<CinemachineFovListener>>();

        if (!_listeners.ContainsKey(channel))
        {
            _listeners[channel] = new List<CinemachineFovListener>();
        }

        return _listeners[channel];
    } 
    
    [SerializeField] private int channel;
    [SerializeField] private float fadeSharpness;

    private float _targetModifier;
    private float _defaultFov;
    private CinemachineVirtualCamera _virtualCamera;

    private void Awake()
    {
        _targetModifier = 1f;
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _defaultFov = _virtualCamera.m_Lens.FieldOfView;
        
        GetListenersList(channel).Add(this);
    }

    private void Update()
    {
        _virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(_virtualCamera.m_Lens.FieldOfView, _defaultFov * _targetModifier, 
            Time.deltaTime * fadeSharpness);
    }

    private void OnDestroy()
    {
        GetListenersList(channel).Remove(this);
    }

    private void SetModifier(float modifier)
    {
        _targetModifier = modifier;
    }
}
