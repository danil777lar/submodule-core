using System;
using UnityEngine;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;

namespace Larje.Core.Services.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MMF_Player))]
    public class FeedbackUIAnimation : MonoBehaviour, IUIPartCloseDelay
    {
        [SerializeField] private bool useForceDelay;
        [SerializeField, MMCondition("useForceDelay")] private float forceDelay;
        [SerializeField] private EventType _eventType;
        private MMF_Player _feedback;

        private void Awake()
        {
            _feedback = GetComponent<MMF_Player>();
            IOpenCloseUI openCloseUI = GetComponentInParent<IOpenCloseUI>(); 
            IShowHideUI showHideUI = GetComponentInParent<IShowHideUI>();

            if (openCloseUI != null) 
            {
                if (_eventType.HasFlag(EventType.Open)) 
                {
                    openCloseUI.Opened += () => _feedback.PlayFeedbacks();
                }
                if (_eventType.HasFlag(EventType.Close))
                {
                    openCloseUI.Closed += () => _feedback.PlayFeedbacks();
                }
            }

            if (showHideUI != null)
            {
                if (_eventType.HasFlag(EventType.Show))
                {
                    showHideUI.Shown += () => _feedback.PlayFeedbacks();
                }
                if (_eventType.HasFlag(EventType.Hide))
                {
                    showHideUI.Hidden += () => _feedback.PlayFeedbacks();
                }
            }
        }

        public float GetDelay()
        {
            if (useForceDelay)
            {
                return forceDelay;
            }
            else
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