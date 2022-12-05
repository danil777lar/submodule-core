using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Larje.Core.Services.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ScreenOpenCloseAnimation : MonoBehaviour, IUIPartCloseDelay
    {
        [SerializeField] private AnimationValuesList _openAnimation;
        [SerializeField] private AnimationValuesList _closeAnimation;

        private CanvasGroup _canvasGroup;
        private RectTransform _rect;

        private CanvasGroup CanvasGroup 
        {
            get 
            {
                if (_canvasGroup == null) 
                {
                    _canvasGroup = GetComponent<CanvasGroup>();
                }
                return _canvasGroup;
            }
        }
        private RectTransform Rect 
        {
            get 
            {
                if (_rect == null) 
                {
                    _rect = (RectTransform)transform;
                }
                return _rect;
            }
        }


        private void Awake()
        {            
            UIScreen screen = GetComponentInParent<UIScreen>();
            UIPopup popup = GetComponentInParent<UIPopup>();
            if (screen != null)
            {
                screen.ScreenOpen += PlayOpenAnimation;
                screen.ScreenClose += PlayCloseAnimation;
            }
            if (popup != null)
            {
                popup.PopupOpened += PlayOpenAnimation;
                popup.PopupClosed += PlayCloseAnimation;
            }
        }

        private void OnDestroy()
        {
            this.DOKill();
        } 


        [ContextMenu("Play Open Animation")]
        public void PlayOpenAnimation(object arguments) 
        {
            if (!Application.isPlaying) 
            {
                return;
            }

            this.DOKill();

            if (_openAnimation.anchoredPosition.use) 
            {
                Vector3 defaultValue = Rect.anchoredPosition;
                Rect.anchoredPosition = _openAnimation.anchoredPosition.value;
                Rect.DOAnchorPos(defaultValue, _openAnimation.anchoredPosition.duration)
                    .SetEase(_openAnimation.anchoredPosition.ease)
                    .SetTarget(this);
            }

            if (_openAnimation.rotation.use)
            {
                Vector3 defaultValue = Rect.rotation.eulerAngles;
                Rect.rotation = Quaternion.Euler(_openAnimation.rotation.value);
                Rect.DOLocalRotate(defaultValue, _openAnimation.rotation.duration)
                    .SetEase(_openAnimation.rotation.ease)
                    .SetTarget(this);
            }

            if (_openAnimation.scale.use)
            {
                Vector3 defaultValue = Rect.localScale;
                Rect.localScale = _openAnimation.scale.value;
                Rect.DOScale(defaultValue, _openAnimation.scale.duration)
                    .SetEase(_openAnimation.scale.ease)
                    .SetTarget(this);
            }

            if (_openAnimation.alpha.use)
            {
                float defaultValue = CanvasGroup.alpha;
                CanvasGroup.alpha = _openAnimation.alpha.value;
                CanvasGroup.DOFade(defaultValue, _openAnimation.alpha.duration)
                    .SetEase(_openAnimation.alpha.ease)
                    .SetTarget(this);
            }
        }

        [ContextMenu("Play Close Animation")]
        public void PlayCloseAnimation() 
        {
            if (!Application.isPlaying)
            {
                return;
            }

            this.DOKill();

            if (_closeAnimation.anchoredPosition.use)
            {
                Rect.DOAnchorPos(_closeAnimation.anchoredPosition.value, _closeAnimation.anchoredPosition.duration)
                    .SetEase(_closeAnimation.anchoredPosition.ease)
                    .SetTarget(this);
            }

            if (_closeAnimation.rotation.use)
            {
                Rect.DOLocalRotate(_closeAnimation.rotation.value, _closeAnimation.rotation.duration)
                    .SetEase(_closeAnimation.rotation.ease)
                    .SetTarget(this);
            }

            if (_closeAnimation.scale.use)
            {
                Rect.DOScale(_closeAnimation.scale.value, _closeAnimation.scale.duration)
                    .SetEase(_closeAnimation.scale.ease)
                    .SetTarget(this);
            }

            if (_closeAnimation.alpha.use)
            {
                CanvasGroup.DOFade(_closeAnimation.alpha.value, _closeAnimation.alpha.duration)
                    .SetEase(_closeAnimation.alpha.ease)
                    .SetTarget(this);
            }
        }

        public float GetDelay()
        {
            List<float> animationDurations = new List<float>();
            animationDurations.Add(_closeAnimation.anchoredPosition.duration);
            animationDurations.Add(_closeAnimation.rotation.duration);
            animationDurations.Add(_closeAnimation.scale.duration);
            animationDurations.Add(_closeAnimation.alpha.duration);
            return animationDurations.Max();
        }


        [Serializable]
        public class AnimationValuesList
        {
            public VectorValueAnimation anchoredPosition;
            public VectorValueAnimation rotation;
            public VectorValueAnimation scale;
            public FloatValueAnimation alpha;
        }

        public abstract class AnimationValues
        {
            public bool use;
            public float duration;
            public Ease ease = Ease.Linear;
        }

        [Serializable]
        public class VectorValueAnimation : AnimationValues
        {
            public Vector3 value;
        }

        [Serializable]
        public class FloatValueAnimation : AnimationValues
        {
            public float value;
        }
    }
}