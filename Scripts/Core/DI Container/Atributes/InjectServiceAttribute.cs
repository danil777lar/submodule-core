using System;

namespace Larje.Core
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InjectServiceAttribute : Attribute
    {
        public InjectServiceAttribute() 
        {

        }
    }
}