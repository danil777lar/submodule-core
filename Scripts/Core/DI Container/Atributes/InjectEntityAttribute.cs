using System;
using Larje.Core.Entities;
using UnityEngine;
using EntityId = Larje.Core.Entities.EntityId;

namespace Larje.Core
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InjectEntityAttribute : Attribute
    {
        public readonly EntityId Id;

        public InjectEntityAttribute(EntityId id)
        {
            Id = id;
        }
    }
}