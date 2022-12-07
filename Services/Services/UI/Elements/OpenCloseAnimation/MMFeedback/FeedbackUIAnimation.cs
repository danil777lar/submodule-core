using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

namespace Larje.Core.Services.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MMF_Player))]
    public class FeedbackUIAnimation : MonoBehaviour, IUIPartCloseDelay
    {
        [SerializeField] private EventType _eventType;
        private MMF_Player _feedback;

        private void Awake()
        {
            _feedback = GetComponent<MMF_Player>();
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
            if (_eventType.HasFlag(EventType.Close) || _eventType.HasFlag(EventType.Hide))
            {
                return _feedback.TotalDuration;
            }
            else 
            {
                return 0f;
            }
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