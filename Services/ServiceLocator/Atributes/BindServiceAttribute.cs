using System;

namespace Larje.Core.Services
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BindServiceAttribute : Attribute
    {
        public Type type;

        public BindServiceAttribute(Type type)
        {
            this.type = type;
        }
    }
}