using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Core.Entities;
using Larje.Core.Services;
using UnityEngine;

namespace Larje.Core
{
    public class DIContainer : MonoBehaviour
    {
        private static DIContainer s_instance;

        public static bool IsInitialized()
        {
            return s_instance != null;
        }
        
        public static T GetService<T>()
        {
            return s_instance._services[typeof(T)].GetComponent<T>();
        }
        
        public static T GetEntity<T>(EntityId id)
        {
            return s_instance._entities[id].FindComponent<T>();
        }

        public static void BindService<T>(Service service)
        {
            if (!s_instance._services.ContainsKey(typeof(T)))
            {
                s_instance._services.Add(typeof(T), service);
            }
        }
        
        public static void BindEntity(Entity entity)
        {
            if (entity.Id == EntityId.None)
                return;
            
            if (!s_instance._entities.ContainsKey(entity.Id))
            {
                s_instance._entities.Add(entity.Id, entity);
            }
        }
        
        public static void UnbindEntity(Entity entity)
        {
            if (entity.Id == EntityId.None)
                return;
            
            if (s_instance._entities.ContainsKey(entity.Id))
            {
                s_instance._entities.Remove(entity.Id);
            }
        }

        public static void InjectTo(object target, Type type = null)
        {
            Type convertedType = type ?? target.GetType();
            FieldInfo[] fields = convertedType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            
            InjectServicesTo(target, fields);
            InjectEntitiesTo(target, fields);
        }

        private static void InjectServicesTo(object target, FieldInfo[] fields)
        {
            FieldInfo[] targetFields = fields.ToList().FindAll((f) => 
                Attribute.IsDefined(f, typeof(InjectServiceAttribute))).ToArray();
           
            foreach (FieldInfo field in targetFields)
            {
                if (s_instance._services.ContainsKey(field.FieldType))
                {
                    MethodInfo getService = typeof(DIContainer)
                        .GetMethod("GetService")
                        ?.MakeGenericMethod(field.FieldType);
                    
                    if (getService != null)
                    {
                        field.SetValue(target, getService.Invoke(s_instance, null));
                    }
                }
            }
        }

        private static void InjectEntitiesTo(object target, FieldInfo[] fields)
        {
            FieldInfo[] targetFields = fields.ToList().FindAll((f) => 
                Attribute.IsDefined(f, typeof(InjectEntityAttribute))).ToArray();
           
            foreach (FieldInfo field in targetFields)
            {
                if (Attribute.GetCustomAttribute(field, typeof(InjectEntityAttribute)) is InjectEntityAttribute attribute)
                {
                    if (attribute.Id != EntityId.None && s_instance._entities.ContainsKey(attribute.Id))
                    {
                        MethodInfo getEntity = typeof(DIContainer)
                            .GetMethod("GetEntity")
                            ?.MakeGenericMethod(field.FieldType);

                        if (getEntity != null)
                        {
                            field.SetValue(target, getEntity.Invoke(s_instance, 
                                new object[] { attribute.Id }));
                        }
                    }
                }
            }
        }
        
            
        [SerializeField] private bool _dontDestroyOnLoad;

        private Dictionary<Type, Service> _services;
        private Dictionary<EntityId, Entity> _entities;

        private void Awake()
        {
            if (s_instance == null)
            {
                s_instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }


            _services = new Dictionary<Type, Service>();
            _entities = new Dictionary<EntityId, Entity>();

            BindChildren();
            InjectChildren();
            InitChildren();
            if (_dontDestroyOnLoad)
            {
                DontDestroyOnLoad(this);
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
                            MethodInfo getService = typeof(DIContainer).GetMethod("GetService")
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