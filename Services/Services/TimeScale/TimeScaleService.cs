using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using ProjectConstants;
using UnityEngine;

namespace Larje.Core.Services
{
    [BindService(typeof(TimeScaleService))]
    public class TimeScaleService : Service
    {
        private const float DEFAULT_FIXED_DELTA_TIME = 0.02f;
        
        private Dictionary<TimeScaleLayerType, float> _timeScaleLayerValues;
        private Dictionary<TimeScaleLayerType, Tween> _timeScaleLayerTween;

        public override void Init()
        {
            InitTimeScaleLayers();
        }

        public void SetTimeScale(TimeScaleLayerType layer, float value)
        {
            _timeScaleLayerValues[layer] = value;
            ApplyTimeScale();
        }

        public float GetTimeScale(TimeScaleLayerType layer)
        {
            if (_timeScaleLayerValues != null && _timeScaleLayerValues.ContainsKey(layer))
            {
                return _timeScaleLayerValues[layer];
            }

            return 1f;
        }

        public float GetTotalTimeScale()
        {
            return Time.timeScale;
        }

        public void PlayTimeScaleAnim(TimeScaleAnimationType type)
        {
            TimeScaleAnimation anim = TimeScaleServiceConfig.Instance.TimeScaleAnimations.ToList()
                .Find(x => x.Type == type);

            if (anim != null)
            {
                _timeScaleLayerTween[anim.Layer]?.Kill();
                if (anim.Duration > 0f)
                {
                    _timeScaleLayerTween[anim.Layer] = DOTween.To(
                            () => 0f,
                            (x) =>
                            {
                                SetTimeScale(anim.Layer, EvaluateAnim(anim, x));
                            },
                            1f, anim.Duration)
                        .SetEase(Ease.Linear)
                        .SetUpdate(UpdateType.Normal, true);
                }
                else
                {
                    SetTimeScale(anim.Layer, EvaluateAnim(anim, 1f));
                }
            }
        }

        private float EvaluateAnim(TimeScaleAnimation anim, float percent)
        {
            return Mathf.Lerp(anim.RemapValues.x, anim.RemapValues.y, anim.Curve.Evaluate(percent));
        }

        private void InitTimeScaleLayers()
        {
            _timeScaleLayerValues = new Dictionary<TimeScaleLayerType, float>();
            _timeScaleLayerTween = new Dictionary<TimeScaleLayerType, Tween>();
            foreach (TimeScaleLayerType layer in Enum.GetValues(typeof(TimeScaleLayerType)))
            {
                _timeScaleLayerValues.Add(layer, 1f);
                _timeScaleLayerTween.Add(layer, null);
            }

            ApplyTimeScale();
        }

        private void ApplyTimeScale()
        {
            float timeScale = 1f;
            _timeScaleLayerValues.Values.ToList().ForEach((x) => timeScale *= x);
            timeScale = Mathf.Max(timeScale, TimeScaleServiceConfig.Instance.MinTimescale);

            Time.timeScale = timeScale;
            Time.fixedDeltaTime = DEFAULT_FIXED_DELTA_TIME * timeScale;
        }
    }
}