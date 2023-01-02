using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Lofelt.NiceVibrations;

namespace Larje.Core.Tools
{
    [CreateAssetMenu(fileName = "Button Interaction Config", menuName = "Configs/Button Interaction Config")]
    public class ButtonInteractionFeedbackConfig : ScriptableObject
    {
        [SerializeField] private ButtonInteractionFeedbackOptions interactable;
        [SerializeField] private ButtonInteractionFeedbackOptions nonInteractable;

        public ButtonInteractionFeedbackOptions Interactable => interactable;
        public ButtonInteractionFeedbackOptions NonInteractable => nonInteractable;
        
        [Serializable]
        public class ButtonInteractionFeedbackOptions
        {
            [Header("Animation")]
            [SerializeField] private float scaleValue = 0.8f;
            [SerializeField] private float inAnimDuration = 0.1f;
            [SerializeField] private float outAnimDuration = 0.25f;
            [SerializeField] private Ease inAnimEase = Ease.Linear;
            [SerializeField] private Ease outAnimEase = Ease.OutBack;
            [Header("Vibration")] 
            [SerializeField] private bool useVibration;
            [SerializeField] private HapticPatterns.PresetType vibrationPreset;
            [Header("Sound")] 
            [SerializeField] private bool useSound;
            [SerializeField] private SoundType soundType;
        
            public float ScaleValue => scaleValue;
            public float InAnimDuration => inAnimDuration;
            public float OutAnimDuration => outAnimDuration;
            public Ease InAnimEase => inAnimEase;
            public Ease OutAnimEase => outAnimEase;
        
            public bool UseVibration => useVibration;
            public HapticPatterns.PresetType VibrationPreset => vibrationPreset;
        
            public bool UseSound => useSound;
            public SoundType SoundType => soundType;    
        }
    }
}