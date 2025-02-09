using System;
using UnityEngine;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;

namespace Larje.Core.Services.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MMF_Player))]
    public class FeedbackUIAnimation : MonoBehaviour, IUIObjectEventDelay
    {
        [SerializeField] private bool useForceDelay;
        [SerializeField, MMCondition("useForceDelay")] private float forceDelay;
        [SerializeField] private EventType _eventType;
        
        private MMF_Player _feedback;
        private MMF_Player Feedback
        {
            get
            {
                if (_feedback == null)
                {
                    _feedback = GetComponent<MMF_Player>();
                }

                return _feedback;
            }
        }

        public float OnOpen()
        {
            return OnEvent(EventType.Open);
        }

        public float OnClose()
        {
            return OnEvent(EventType.Close);
        }

        public float OnShow()
        {
            return OnEvent(EventType.Show);
        }

        public float OnHide()
        {
            return OnEvent(EventType.Hide);
        }

        public float OnFocus()
        {
            return OnEvent(EventType.Focus);
        }

        public float OnUnfocus()
        {
            return OnEvent(EventType.Unfocus);
        }
        
        private float OnEvent(EventType eventType)
        {
            if (_eventType.HasFlag(eventType))
            {
                if (Feedback.IsPlaying)
                {
                    Feedback.StopFeedbacks();
                }
                
                Feedback.PlayFeedbacks();
                return GetDelay();
            }
            else
            {
                return 0f;
            }
        }

        private void OnDisable()
        {
            if (Feedback.IsPlaying)
            {
                Feedback.StopFeedbacks();
                Feedback.Revert();
            }
        }

        private float GetDelay()
        {
            return useForceDelay ? forceDelay : Feedback.TotalDuration;
        }

        [Flags]
        private enum EventType 
        {
            Open = 1, 
            Close = 2, 
            Show = 4,
            Hide = 8,
            Focus = 16,
            Unfocus = 32
        }
    }
}