using System.Collections.Generic;
using Larje.Core;
using Larje.Core.Entities;
using UnityEngine;
using EntityId = Larje.Core.Entities.EntityId;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class ColliderTriggerSender : TriggerSender
{
    [Space, Header("Collider Settings")] 
    [SerializeField] private bool onlyPlayer;
    [SerializeField] private LayerMask mask;

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
        if (onlyPlayer)
        {
            if (_player == null)
            {
                _player = DIContainer.GetEntityComponent<Collider>(EntityId.Player).gameObject;
            }
            return _player != null && other.gameObject == _player;
        }
        return mask.HasLayer(other.gameObject.layer);
    }
}
