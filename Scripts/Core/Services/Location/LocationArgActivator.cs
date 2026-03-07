using System;
using System.Linq;
using Larje.Core;
using ProjectConstants;
using UnityEngine;

public class LocationArgActivator : MonoBehaviour
{
    [SerializeField] private bool permitted = true;
    [SerializeField] private Option[] options;
    
    [InjectService] private LocationService _locationService;
    [InjectService] private GameEventService _gameEventService;

    private void Start()
    {
        DIContainer.InjectTo(this);

        if (permitted)
        {
            foreach (Option option in options)
            {
                bool isActive = _locationService.CurrentLocation.LocationArgs.Contains(option.ArgType);
                option.TargetObject.SetActive(isActive);
            }
        }
    }

    private void OnValidate()
    {
        if (options == null || options.Length == 0)
        {
            return;
        }

        foreach (Option option in options)
        {
            option.OnValidate();
        }
    }

    [Serializable]
    private class Option
    {
        [HideInInspector, SerializeField] public string inspectorName;

        [field: SerializeField] public LocationArgType ArgType { get; private set; }
        [field: SerializeField] public GameObject TargetObject { get; private set; }

        public void OnValidate()
        {
            inspectorName = ArgType.ToString();
        }
    }
}
