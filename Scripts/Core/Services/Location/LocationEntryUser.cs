using System.Collections;
using System.Collections.Generic;
using Larje.Core;
using UnityEngine;

public class LocationEntryUser : MonoBehaviour
{
    [InjectService] private LocationService _locationService; 
    
    private void Start()
    {
        DIContainer.InjectTo(this);

        CharacterTransformData dataUser = GetComponent<CharacterTransformData>();

        if (dataUser == null || !dataUser.DataInjected)
        {
            if (_locationService.TryGetLocationEntry(out ILocationEntry entry))
            {
                transform.position = entry.Position;
                transform.forward = entry.Direction;
            }
        }
    }
}
