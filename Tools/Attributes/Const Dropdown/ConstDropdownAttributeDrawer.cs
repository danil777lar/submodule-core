using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            Type fieldType = property.serializedObject.targetObject.GetType().GetField(property.name).FieldType;
            FieldInfo targetField = targetObject.GetType().GetField(property.name); 
            
            FieldInfo[] fieldInfos = GetConstants(constHolderType, fieldType);
            Dictionary<string, object> fieldValues = GetFieldValues(fieldInfos);
            List<string> keys = new List<string>();
            fieldValues.Keys.ToList().ForEach(x => keys.Add(x));
            List<object> values = new List<object>();
            fieldValues.Values.ToList().ForEach(x => values.Add(x));

            int index = 0;
            if (fieldValues.Values.Contains(targetField.GetValue(targetObject)))
            {
                index = values.IndexOf(targetField.GetValue(targetObject));
            }
            index = EditorGUI.Popup(position, index, keys.ToArray());
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