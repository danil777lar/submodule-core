using System.Collections;
using System.Collections.Generic;
using Larje.Core;
using UnityEngine;

public class LocationEntry : MonoBehaviour, ILocationEntry
{
    [field: SerializeField] public int Id { get; private set; }

    [InjectService] private LocationService _locationService;
    
    public Vector3 Position => transform.position;
    public Vector3 Direction => transform.forward;
    
    private void Awake()
    {
        DIContainer.InjectTo(this);
        _locationService.AddLocationEntry(this);
    }

    private void OnDestroy()
    {
        _locationService.RemoveLocationEntry(this);
    }
}
