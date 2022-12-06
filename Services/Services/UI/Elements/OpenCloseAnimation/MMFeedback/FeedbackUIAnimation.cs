using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

namespace Larje.Core.Services.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MMFeedbacks))]
    public class FeedbackUIAnimation : MonoBehaviour, IUIPartCloseDelay
    {
        [SerializeField] private EventType _eventType;
        private MMFeedbacks _feedback;

        private void Awake()
        {
            _feedback = GetComponent<MMFeedbacks>();
            UIScreen screen = GetComponentInParent<UIScreen>();
            UIPopup popup = GetComponentInParent<UIPopup>();

            if (screen != null) 
            {
                if (_eventType.HasFlag(EventType.Open)) 
                {
                    screen.ScreenOpen += (_) => _feedback.PlayFeedbacks();
                }
                if (_eventType.HasFlag(EventType.Close))
                {
                    screen.ScreenClose += () => _feedback.PlayFeedbacks();
                }
            }

            if (popup != null)
            {
                if (_eventType.HasFlag(EventType.Open))
                {
                    popup.PopupOpened += (_) => _feedback.PlayFeedbacks();
                }
                if (_eventType.HasFlag(EventType.Close))
                {
                    popup.PopupClosed += () => _feedback.PlayFeedbacks();
                }
                if (_eventType.HasFlag(EventType.Show))
                {
                    popup.PopupShowed += () => _feedback.PlayFeedbacks();
                }
                if (_eventType.HasFlag(EventType.Hide))
                {
                    popup.PopupHidden += () => _feedback.PlayFeedbacks();
                }
            }
        }

        public float GetDelay()
        {
            return _feedback.TotalDuration;
        }


        [Flags]
        private enum EventType 
        {
            Open = 1, 
            Close = 2, 
            Show = 4,
            Hide = 8
        }
    }
}