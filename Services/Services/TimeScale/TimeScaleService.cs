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
            TimeScaleAnimation animation = TimeScaleServiceConfig.Instance.TimeScaleAnimations.ToList()
                .Find(x => x.Type == type);

            if (animation != null)
            {
                _timeScaleLayerTween[animation.Layer]?.Kill();
                _timeScaleLayerTween[animation.Layer] = DOTween.To(
                        () => 0f,
                        (x) =>
                        {
                            float value = Mathf.Lerp(animation.RemapValues.x, animation.RemapValues.y, 
                                animation.Curve.Evaluate(x));
                            SetTimeScale(animation.Layer, value);
                        },
                        1f, animation.Duration)
                    .SetUpdate(UpdateType.Normal, true);
            }
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