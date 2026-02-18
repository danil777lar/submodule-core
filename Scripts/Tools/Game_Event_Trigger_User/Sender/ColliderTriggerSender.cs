using System.Collections.Generic;
using Larje.Core;
using Larje.Core.Entities;
using UnityEngine;
using EntityId = Larje.Core.Entities.EntityId;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class ColliderTriggerSender : TriggerSender
{
    [Space, Header("Collider Settings")] 
    [SerializeField] private bool useLayerMask = false;
    [SerializeField] private LayerMask layerMask;
    [Space]
    [SerializeField] private bool useEntityMask = false;
    [SerializeField] private List<EntityId> entityMask = new List<EntityId>();

    private GameObject _player;
    private List<GameObject> _interactables = new List<GameObject>();
    
    private void Update()
    {
        Value = _interactables.Count > 0 ? 1f : 0f;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (IsInteractable(other) && !_interactables.Contains(other.gameObject))
        {
            _interactables.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsInteractable(other) && _interactables.Contains(other.gameObject))
        {
            _interactables.Remove(other.gameObject);
        }
    }
    
    private bool IsInteractable(Collider other)
    {
        bool result = true;

        if (useLayerMask)
        {
            result &= layerMask.HasLayer(other.gameObject.layer);
        }

        if (useEntityMask)
        {
            bool entityMaskOk = false;
            foreach (EntityId entityId in entityMask)
            {
                Collider foundCollider = DIContainer.GetEntityComponent<Collider>(entityId);
                entityMaskOk |= foundCollider != null && foundCollider == other;
            }
            result &= entityMaskOk;
        }

        return result;
    }
}
