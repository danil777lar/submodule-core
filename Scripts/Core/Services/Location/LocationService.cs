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
    [SerializeField] private LocationType defaultLocationType;
    [SerializeField] private int defaultLocationEntry = 0;
    [Space]
    [SerializeField] private float transitionDuration = 0.5f;
    [Space]
    [SerializeField] private List<LocationInfo> locations;
    
    [InjectService] private IDataService _dataService;
    [InjectService] private IGameStateService _gameStateService;
    [InjectService] private BootstrapperService _bootstrapperService;

    private float _transitionValue;
    private List<ILocationEntry> _locationEntries = new List<ILocationEntry>();
    private List<CallbackData> _locationCallbacks = new List<CallbackData>();

    public float TransitionValue => _transitionValue;

    public LocationInfo CurrentLocation
    {
        get => locations.Find(x => x.LocationType == _dataService.GameData.LocationData.CurrentLocation);
        private set => _dataService.GameData.LocationData.CurrentLocation = value.LocationType;
    }
    
    public int CurrentLocationEntry
    {
        get => _dataService.GameData.LocationData.CurrentLocationEntry;
        private set => _dataService.GameData.LocationData.CurrentLocationEntry = value;
    }

    public event Action EventExitLocation;
    public event Action EventStartLoadLocation;
    public event Action<LocationType, int> EventFinishLoadLocation;
    public event Action<LocationType, int> EventLocationEntered;  
    
    public override void Init()
    {
        if (!_dataService.GameData.LocationData.Inited)
        {
            CurrentLocation = locations.Find(x => x.LocationType == defaultLocationType);
            CurrentLocationEntry = defaultLocationEntry;

            _dataService.GameData.LocationData.Inited = true;
            _dataService.EventAfterLoad += OnDataLoaded;
        }
    }
    
    public void LoadLocation(LocationType locationType, int entryId = 0)
    {
        LocationInfo locationInfo = locations.Find(x => x.LocationType == locationType);
        if (locationInfo != null)
        {
            CurrentLocation = locationInfo;
            CurrentLocationEntry = entryId;

            EventStartLoadLocation?.Invoke();
            DOTween.To(() => _transitionValue, value => _transitionValue = value, 1f, transitionDuration)
                .OnComplete(() =>
                {
                    EventExitLocation?.Invoke();
                    _bootstrapperService.LoadSceneAsync(locationInfo.SceneName, () =>
                    {
                        _locationCallbacks.ForEach(TryCallCallback);
                        ApplyLightmaps();

                        EventFinishLoadLocation?.Invoke(locationType, entryId);
                        DOVirtual.Float(1f, 0f, transitionDuration, value => _transitionValue = value)
                        .OnComplete(() =>
                        {
                            _gameStateService.SetGameState(GameStates.Playing);
                            EventLocationEntered?.Invoke(locationType, entryId); 
                        });
                    });
                });
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

    public void ApplyLightmaps()
    {
        Texture2D[] lightmapColors = CurrentLocation.Lightmaps.ToArray();

        if (lightmapColors.Length > 0)
        {
            LightmapData[] newMaps = new LightmapData[lightmapColors.Length];
            for (int i = 0; i < newMaps.Length; i++)
            {
                newMaps[i] = new LightmapData();
                newMaps[i].lightmapColor = lightmapColors[i];
            }
            LightmapSettings.lightmaps = newMaps;
        }
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
        bool callNow = callbackData.anyLocation || CurrentLocation.LocationType == callbackData.locationType;
        callNow &= callbackData.entryId < 0 || CurrentLocationEntry == callbackData.entryId;
        callNow &= callbackData.args == null || callbackData.args.Count == 0 || callbackData.args.TrueForAll(x => CurrentLocation.LocationArgs.Contains(x));

        if (callNow)
        {
            callbackData.action?.Invoke();
        }
    }

    private void OnDataLoaded()
    {
        LoadLocation(CurrentLocation.LocationType, CurrentLocationEntry);
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
    public class LocationInfo
    {
        [HideInInspector, SerializeField] public string inspectorName;

        [SerializeField] private LocationType locationType;
        [SerializeField] private string sceneName;
        [SerializeField] private List<LocationArgType> locationArgs;
        [SerializeField] private List<Texture2D> lightmaps;
        
        public LocationType LocationType => locationType;
        public string SceneName => sceneName;

        public IReadOnlyCollection<LocationArgType> LocationArgs => locationArgs;
        public IReadOnlyCollection<Texture2D> Lightmaps => lightmaps;

        public void Validate()
        {
            inspectorName = locationType.ToString();
        }
    }
}
