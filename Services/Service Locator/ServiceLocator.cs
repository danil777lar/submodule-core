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

        private static ServiceLocator _instance;
        public static ServiceLocator Instance => _instance;

        #endregion

        [SerializeField] private bool _dontDestroyOnLoad;

        private Dictionary<Type, Service> _services;


        private void Awake()
        {
            _instance = this;
            _services = new Dictionary<Type, Service>();

            BindChildren();
            InjectChildren();
            InitChildren();
            if (_dontDestroyOnLoad)
            {
                DontDestroyOnLoad(this);
            }
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

        public void InjectServicesInComponent(Component component, Type type = null)
        {
            List<FieldInfo> fields = new List<FieldInfo>();

            if (type != null)
            {
                fields.AddRange(type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
            }
            else
            {
                fields.AddRange(component.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
            }

            fields = fields.FindAll((f) => Attribute.IsDefined(f, typeof(InjectServiceAttribute)));
            foreach (FieldInfo field in fields)
            {
                if (_services.ContainsKey(field.FieldType))
                {
                    MethodInfo getService = typeof(ServiceLocator).GetMethod("GetService")
                        .MakeGenericMethod(field.FieldType);
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
                    Attribute attr = Attribute.GetCustomAttribute(service.GetType(), typeof(BindServiceAttribute));
                    if (attr is BindServiceAttribute attribute)
                    {
                        foreach (Type type in attribute.type)
                        {
                            if (!_services.ContainsKey(type))
                            {
                                _services.Add(type, service);       
                            }
                        }
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
                    List<FieldInfo> fields =
                        new List<FieldInfo>(service.GetType()
                            .GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
                    fields = fields.FindAll((f) => Attribute.IsDefined(f, typeof(InjectServiceAttribute)));
                    foreach (FieldInfo field in fields)
                    {
                        if (_services.ContainsKey(field.FieldType))
                        {
                            MethodInfo getService = typeof(ServiceLocator).GetMethod("GetService")
                                .MakeGenericMethod(field.FieldType);
                            field.SetValue(service, getService.Invoke(this, null));
                        }
                    }
                }
            }
        }

        private void InitChildren()
        {
            foreach (Transform child in transform)
            {
                Service service = child.gameObject.GetComponent<Service>();
                if (service)
                {
                    service.Init();
                }
            }
        }
    }
}