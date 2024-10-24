using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core;
using Larje.Core.Entities;
using UnityEditor;
using UnityEngine;

namespace Larje.Core
{
    [CustomEditor(typeof(DIContainer))]
    public class DIContainerEditor : Editor
    {
        private DIContainer _container;

        private void OnEnable()
        {
            _container = (DIContainer) target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (_container == null)
            {
                return;
            }
            
            if (_container.Services != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Services", EditorStyles.toolbarTextField);
                EditorGUILayout.Space();
                
                Dictionary <Service, List<Type>> services = new Dictionary<Service, List<Type>>();
                foreach (KeyValuePair<Type, Service> service in _container.Services)
                {
                    if (services.ContainsKey(service.Value))
                    {
                        services[service.Value].Add(service.Key);
                    }
                    else
                    {
                        services.Add(service.Value, new List<Type> {service.Key});
                    }
                }
                
                foreach (KeyValuePair<Service, List<Type>> service in services)
                {
                    EditorGUILayout.LabelField(service.Key.name, EditorStyles.whiteLabel);
                    foreach (Type type in service.Value)
                    {
                        EditorGUILayout.LabelField($"     {type.Name}");
                    }
                    EditorGUILayout.Space();
                }
            }

            if (_container.Entities != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Entities", EditorStyles.toolbarTextField);
                EditorGUILayout.Space();

                foreach (KeyValuePair<EntityId, Entity> entity in _container.Entities)
                {
                    if (EditorGUILayout.LinkButton($"{entity.Key.ToString()} - {entity.Value.name}"))
                    {
                        Selection.activeGameObject = entity.Value.gameObject;
                    }
                    EditorGUILayout.Space();
                }
            }
        }
    }
}