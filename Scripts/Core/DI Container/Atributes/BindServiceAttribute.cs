using System;

namespace Larje.Core
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