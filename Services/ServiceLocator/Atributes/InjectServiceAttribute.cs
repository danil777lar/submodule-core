using System;

namespace Larje.Core.Services
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InjectServiceAttribute : Attribute
    {
        public InjectServiceAttribute() 
        {

        }
    }
}