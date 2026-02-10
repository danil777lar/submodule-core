using System.Linq;
using Larje.Core;
using ProjectConstants;
using UnityEngine;

public class LocationArgActivator : MonoBehaviour
{
    [SerializeField] private LocationArgType argType;
    [SerializeField] private GameObject targetObject;
    [Space]
    [SerializeField] private string triggerName;
    
    [InjectService] private LocationService _locationService;

    private void Start()
    {
        DIContainer.InjectTo(this);
        
        bool isActive = _locationService.CurrentLocation.LocationArgs.Contains(argType);
        targetObject.SetActive(isActive);

        if (isActive)
        {
            TriggerRoot root = GetComponentInParent<TriggerRoot>();
            root.ValueChangedBool(triggerName, true);
            root.ValueChangedFloat(triggerName, 1f);
        }

        Debug.Log($"{triggerName} {isActive}");
    }
}
