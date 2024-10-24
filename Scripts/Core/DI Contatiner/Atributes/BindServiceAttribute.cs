using System;

namespace Larje.Core.Services
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BindServiceAttribute : Attribute
    {
        public Type[] type;

        public BindServiceAttribute(params Type[] type)
        {
            this.type = type;
        }
    }
}