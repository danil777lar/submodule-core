using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Larje.Core;
using Larje.Core.Services;
using ProjectConstants;
using UnityEngine;

[BindService(typeof(LocationService))]
public class LocationService : Service
{
    [SerializeField] private LocationType defaultLocationType = LocationType.Office;
    [SerializeField] private int defaultLocationEntry = 0;
    [SerializeField] private List<LocationArgType> defaultLocationArguments = new List<LocationArgType>();
    [Space]
    [SerializeField] private List<LocationInfo> locations;
    [Space]
    [SerializeField] private float transitionDuration = 0.5f;
    
    [InjectService] private IDataService _dataService;
    [InjectService] private BootstrapperService _bootstrapperService;

    private float _transitionValue;
    private List<ILocationEntry> _locationEntries = new List<ILocationEntry>();
    private List<CallbackData> _locationCallbacks = new List<CallbackData>();

    public LocationType CurrentLocation
    {
        get => _dataService.GameData.LocationData.CurrentLocation;
        private set => _dataService.GameData.LocationData.CurrentLocation = value;
    }
    
    public int CurrentLocationEntry
    {
        get => _dataService.GameData.LocationData.CurrentLocationEntry;
        private set => _dataService.GameData.LocationData.CurrentLocationEntry = value;
    }
    
    public IReadOnlyCollection<LocationArgType> CurrentArguments
    {
        get => _dataService.GameData.LocationData.CurrentArguments;
        private set => _dataService.GameData.LocationData.CurrentArguments = new List<LocationArgType>(value);
    }

    public event Action EventStartLoadLocation;
    public event Action<LocationType, int> EventLocationEntered;  
    
    public override void Init()
    {
        if (!_dataService.GameData.LocationData.Inited)
        {
            CurrentLocation = defaultLocationType;
            CurrentLocationEntry = defaultLocationEntry;
            CurrentArguments = defaultLocationArguments;

            _dataService.GameData.LocationData.Inited = true;
            _dataService.EventAfterLoad += OnDataLoaded;
        }
    }
    
    public void LoadLocation(LocationType locationType, int entryId = 0, List<LocationArgType> locationArgType = null)
    {
        LocationInfo locationInfo = locations.Find(x => x.LocationType == locationType);
        if (locationInfo != null)
        {
            CurrentLocation = locationType;
            CurrentLocationEntry = entryId;
            CurrentArguments = locationArgType ?? new List<LocationArgType>();

            if (LarjePostFXFeature.TryGetFX(out LarjeFXTransition.Processor transitionFX))
            {
                transitionFX.AddProvider(GetTransitionValue);
            }
            DOVirtual.Float(0f, 1f, transitionDuration, value => _transitionValue = value)
                .OnComplete(() =>
                {
                    _bootstrapperService.LoadLocation(locationInfo.SceneName, () =>
                    {
                        _locationCallbacks.ForEach(TryCallCallback);
                        EventLocationEntered?.Invoke(locationType, entryId); 
                        DOVirtual.Float(1f, 0f, transitionDuration, value => _transitionValue = value);
                    });
                });

            EventStartLoadLocation?.Invoke();
        }
    }
    
    public void AddLocationEntry(ILocationEntry entry)
    {
        ILocationEntry storedEntry = _locationEntries.Find(x => x.Id == entry.Id);
        if (storedEntry != null)
        {
            _locationEntries.Remove(storedEntry);
        }
        
        if (!_locationEntries.Contains(entry))
        {
            _locationEntries.Add(entry);
        }
    }
    
    public void RemoveLocationEntry(ILocationEntry locationEntry)
    {
        _locationEntries.Remove(locationEntry);
    }

    public bool TryGetLocationEntry(out ILocationEntry entry)
    {
        entry = _locationEntries.Find(x => x.Id == CurrentLocationEntry);
        return entry != null;
    }

    public void AddLocationEnterCallback(CallbackData callbackData)
    {
        TryCallCallback(callbackData);
        _locationCallbacks.Add(callbackData);
    }

    public void RemoveLocationEnterCallback(object target)
    {
        _locationCallbacks.RemoveAll(x => x.target == target);
    }

    private void OnValidate()
    {
        foreach (LocationInfo location in locations)
        {
            location.Validate();
        }
    }

    private void TryCallCallback(CallbackData callbackData)
    {
        bool callNow = callbackData.anyLocation || CurrentLocation == callbackData.locationType;
        callNow &= callbackData.entryId < 0 || CurrentLocationEntry == callbackData.entryId;
        callNow &= callbackData.args == null || callbackData.args.Count == 0 || callbackData.args.TrueForAll(x => CurrentArguments.Contains(x));

        if (callNow)
        {
            callbackData.action?.Invoke();
        }
    }

    private void OnDataLoaded()
    {
        LoadLocation(CurrentLocation, CurrentLocationEntry, new List<LocationArgType>(CurrentArguments));
    }

    private float GetTransitionValue()
    {
        return _transitionValue;
    }

    public class CallbackData
    {
        public object target;
        public Action action;

        public bool anyLocation = false;
        public LocationType locationType; 
        public int entryId = -1;
        public List<LocationArgType> args = null; 

        public CallbackData(object target, LocationType location, Action action)
        {
            this.target = target;
            this.locationType = location;
            this.action = action;
        }

        public CallbackData(object target, Action action)
        {
            this.target = target;
            this.action = action;
            this.anyLocation = true;
        }
    }
    
    [Serializable]
    private class LocationInfo
    {
        [HideInInspector, SerializeField] public string inspectorName;
        [field: SerializeField] public LocationType LocationType { get; private set; }
        [field: SerializeField] public string SceneName { get; private set; }
        
        public void Validate()
        {
            inspectorName = LocationType.ToString();
        }
    }
}
