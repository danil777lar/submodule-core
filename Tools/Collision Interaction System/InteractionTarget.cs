using System;
using UnityEngine;
using MoreMountains.Feedbacks;
using Larje.Core.Tools.Interaction.Processors;
using ProjectConstants;

namespace Larje.Core.Tools.Interaction
{
    public abstract class InteractionTarget : MonoBehaviour, IInteractionTarget
    {
        [SerializeField] private InteractionTargetType targetType;
        [SerializeField] private InteractionProcessorType interactionProcessorType;
        [SerializeField, MMFEnumCondition("interactionProcessorType", 1)] protected int fullHealth;
        [SerializeField, MMFEnumCondition("interactionProcessorType", 2)] protected int hitsToKill;
        [Header("Feedbacks")]
        [SerializeField] protected MMF_Player interactionFeedback;
        [SerializeField] protected MMF_Player completeFeedback;
        
        private InteractionProcessor _interactionProcessor;

        public event Action Interacted;
        public event Action Completed;

        public void TryInteract(InteractionData data)
        {
            _interactionProcessor.ProcessInteraction(data);
        }

        public float GetInteractionProgress()
        {
            return _interactionProcessor.GetProgress();
        }

        public InteractionTargetType GetTargetType()
        {
            return targetType;
        }

        protected virtual void Awake()
        {
            switch (interactionProcessorType)
            {
                case InteractionProcessorType.Immortal:
                    _interactionProcessor = new ImmortalInteractionProcessor(OnInteracted, OnCompleted);
                    break;
                case InteractionProcessorType.HealthCount:
                    _interactionProcessor = new HealthInteractionProcessor(fullHealth, OnInteracted, OnCompleted);
                    break;
                case InteractionProcessorType.HitCount:
                    _interactionProcessor = new HitCountInteractionProcessor(hitsToKill, OnInteracted, OnCompleted);
                    break;
            }
        }

        protected virtual void OnInteracted()
        {
            Interacted?.Invoke();
            if (interactionFeedback != null)
            {
                interactionFeedback.PlayFeedbacks();
            }
        }

        protected virtual void OnCompleted()
        {
            Completed?.Invoke();
            if (completeFeedback != null)
            {
                completeFeedback.PlayFeedbacks();
            }
        }

        private enum InteractionProcessorType
        {
            Immortal = 0,
            HealthCount = 1,
            HitCount = 2
        }
    }
}