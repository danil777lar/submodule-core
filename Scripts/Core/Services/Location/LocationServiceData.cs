using System;
using System.Collections.Generic;
using ProjectConstants;
using UnityEngine;

namespace Larje.Core.Services
{
    public partial class GameData
    {
        public LocationServiceData LocationData;
    }
}

[Serializable]
public class LocationServiceData
{
    public bool Inited = false;
    public LocationType CurrentLocation;
    public int CurrentLocationEntry = 0;
    public List<LocationArgType> CurrentArguments = new List<LocationArgType>();
}
