using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using UnityEngine;

namespace Larje.Core.Entities
{
    public class Entity : MonoBehaviour
    {
        [field: SerializeField] public EntityId Id { get; private set; }

        private void Awake()
        {

        }
    }
}