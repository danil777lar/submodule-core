using System.Linq;
using Larje.Core;
using ProjectConstants;
using UnityEngine;

public class LocationArgActivator : MonoBehaviour
{
    [SerializeField] private LocationArgType argType;
    [SerializeField] private GameObject targetObject;
    [Space]
    [SerializeField] private TriggerConstant trigger;
    
    [InjectService] private LocationService _locationService;
    [InjectService] private GameEventService _gameEventService;

    private void Start()
    {
        DIContainer.InjectTo(this);

        bool isActive = _locationService.CurrentLocation.LocationArgs.Contains(argType);
        targetObject?.SetActive(isActive);

        if (isActive && trigger != null)
        {
            _gameEventService.SendEvent(new GameEventTrigger(trigger, 1f, gameObject.name));
        }
    }
}
