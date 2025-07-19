using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Tools
{
    [CreateAssetMenu(fileName = "Button Interaction Config", menuName = "Larje/Core/Tools/Button Interaction Config")]
    public class ButtonInteractionFeedbackConfig : ScriptableObject
    {
        [SerializeField] private Material material;
        [Space] 
        [SerializeField] private List<ButtonInteractionState> states;
            
        public Material Material => material;
        public IReadOnlyCollection<ButtonInteractionState> States => states;

        private void OnValidate()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }
    }
}