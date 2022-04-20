using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services
{
    public class ServiceLocator : MonoBehaviour
    {
        #region Default
        private static ServiceLocator _default;
        public static ServiceLocator Default => _default;
        #endregion

        private Dictionary<Type, Service> _services;


        private void Awake()
        {
            _services = new Dictionary<Type, Service>();

            BindChildren();
            InjectChildren();
        }


        public T GetService<T>()
        {
            return _services[typeof(T)].GetComponent<T>();
        }

        public void BindService<T>(Service service) 
        {
            if (!_services.ContainsKey(typeof(T))) 
            {
                _services.Add(typeof(T), service);
            }
        }

        public void InjectServicesInComponent(Component component) 
        {
            List<FieldInfo> fields = new List<FieldInfo>(component.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
            fields = fields.FindAll((f) => Attribute.IsDefined(f, typeof(InjectServiceAttribute)));
            foreach (FieldInfo field in fields)
            {
                if (_services.ContainsKey(field.FieldType))
                {
                    MethodInfo getService = typeof(ServiceLocator).GetMethod("GetService").MakeGenericMethod(field.FieldType);
                    field.SetValue(component, getService.Invoke(this, null));
                }
            }
        }


        private void BindChildren()
        {
            foreach (Transform child in transform)
            {
                Service service = child.gameObject.GetComponent<Service>();
                if (service)
                {
                    BindServiceAttribute attribute = Attribute.GetCustomAttribute(service.GetType(), typeof(BindServiceAttribute)) as BindServiceAttribute;

                    if (attribute != null && !_services.ContainsKey(attribute.type)) 
                    {
                        _services.Add(attribute.type, service);
                        service.Init();
                    }
                }
            }
        }

        private void InjectChildren() 
        {
            foreach (Transform child in transform)
            {
                Service service = child.gameObject.GetComponent<Service>();
                if (service)
                {
                    List<FieldInfo> fields = new List<FieldInfo>(service.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
                    fields = fields.FindAll((f) => Attribute.IsDefined(f, typeof(InjectServiceAttribute)));
                    foreach (FieldInfo field in fields) 
                    {
                        if (_services.ContainsKey(field.FieldType))
                        {
                            MethodInfo getService = typeof(ServiceLocator).GetMethod("GetService").MakeGenericMethod(field.FieldType);
                            field.SetValue(service, getService.Invoke(this, null));
                        }
                    }
                }
            }
        }
    }
}
