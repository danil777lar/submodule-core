using System.Collections;
using System.Collections.Generic;
using Larje.Core;
using UnityEngine;

public class LocationEntryUser : MonoBehaviour
{
    public bool permitted = true;

    [InjectService] private LocationService _locationService; 
    
    private void Start()
    {
        DIContainer.InjectTo(this);

        if (permitted)
        {
            if (_locationService.TryGetLocationEntry(out ILocationEntry entry))
            {
                transform.position = entry.Position;
                transform.forward = entry.Direction;
            }
        }
    }
}
