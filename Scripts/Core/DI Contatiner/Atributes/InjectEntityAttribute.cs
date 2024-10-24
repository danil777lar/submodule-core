using System;
using Larje.Core.Entities;

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