using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Larje.Core.Services;
using Larje.Core.Tools;
using UnityEngine;
using UnityEditor;

namespace Larje.Core.Tools
{
    [CustomPropertyDrawer(typeof(CustomEnumAttribute))]
    public class CustomEnumAttributePropertyDrawer : PropertyDrawer
    {
#if UNITY_EDITOR
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            object targetObject = property.serializedObject.targetObject;
            Type fieldType = property.serializedObject.targetObject.GetType().GetField(property.name).FieldType;
            FieldInfo targetField = targetObject.GetType().GetField(property.name);
            FieldInfo[] constants = GetConstants(fieldType);
            Dictionary<string, object> fieldValues = GetFieldValues(constants);
            
            int index = 0;
            if (fieldValues.Values.Contains(targetField.GetValue(targetObject)))
            {
                index = fieldValues.Values.ToList().IndexOf(targetField.GetValue(targetObject));
            }

            EditorGUI.LabelField(new Rect(position.x, position.y, position.width / 2f, position.height), label);
            index = EditorGUI.Popup(new Rect(position.x + (position.width / 2f), position.y, position.width / 2f, position.height), index, fieldValues.Keys.ToArray());
            targetField.SetValue(targetObject, constants[index].GetValue(null));    
            property.serializedObject.Update();
            property.serializedObject.ApplyModifiedProperties();
        }

        private FieldInfo[] GetConstants(Type targetClass)
        {
            List<FieldInfo> constants = new List<FieldInfo>();
            FieldInfo[] fieldInfos = targetClass.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (FieldInfo fi in fieldInfos)
            {
                if (fi.FieldType == targetClass)
                {
                    constants.Add(fi);
                }
            }

            return constants.ToArray();
        }

        private Dictionary<string, object> GetFieldValues(FieldInfo[] fields)
        {
            Dictionary<string, object> fieldValues = new Dictionary<string, object>();
            foreach (FieldInfo field in fields)
            {
                fieldValues.Add(field.Name, field.GetValue(null));
            }

            return fieldValues;
        }
#endif
    }
}