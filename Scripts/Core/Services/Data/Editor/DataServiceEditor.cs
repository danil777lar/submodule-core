using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Larje.Core.Services
{
    [CustomEditor(typeof(DataService))]
    public class DataServiceEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}