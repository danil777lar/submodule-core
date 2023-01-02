using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Larje.Core.Tools
{
    [CustomPropertyDrawer(typeof(ConstDropdownAttribute))]
    public class ConstDropdownAttributeDrawer : PropertyDrawer
    {
#if UNITY_EDITOR
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            object targetObject = property.serializedObject.targetObject;
            Type constHolderType = (attribute as ConstDropdownAttribute).targetClass;
            FieldInfo targetField = targetObject.GetType().GetField(property.name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (targetField == null)
            {
                return;
            }

            FieldInfo[] fieldInfos = GetConstants(constHolderType, targetField.FieldType);
            Dictionary<string, object> fieldValues = GetFieldValues(fieldInfos);
            
            int index = 0;
            if (fieldValues.Values.Contains(targetField.GetValue(targetObject)))
            {
                index = fieldValues.Values.ToList().IndexOf(targetField.GetValue(targetObject));
            }
            
            EditorGUI.LabelField(new Rect(position.x, position.y, position.width / 2f, position.height), label);
            index = EditorGUI.Popup(new Rect(position.x + (position.width / 2f), position.y, position.width / 2f, position.height), index, fieldValues.Keys.ToArray());
            targetField.SetValue(targetObject, fieldInfos[index].GetValue(null));
    }
        
        private FieldInfo[] GetConstants(Type targetClass, Type fieldType)
        {
            List<FieldInfo> constants = new List<FieldInfo>();
            FieldInfo[] fieldInfos = targetClass.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            foreach (FieldInfo fi in fieldInfos)
            {
                if (fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == fieldType)
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