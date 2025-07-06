using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Larje.Core.Services;
using UnityEngine;
using Lofelt.NiceVibrations;
using ProjectConstants;
using SoundType = Larje.Core.Services.SoundType;

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
    }
}