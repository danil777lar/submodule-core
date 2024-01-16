using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderBlink : MonoBehaviour
{
    [SerializeField] private Vector3 addScale;
    [Header("Blink")]
    [SerializeField] private float blinkDuration;
    [SerializeField, Range(0f, 1f)] private float blinkStart;
    [Header("Curves")]
    [SerializeField] private AnimationCurve colorLerpCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private AnimationCurve blinkFrequencyCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private AnimationCurve blinkValueCurve;
    [Header("Colors")]
    [SerializeField] private Color minColor;
    [SerializeField] private Color maxColor;
    [SerializeField] private Color blinkColor;

    private bool _blinkStarted;
    private float _blinkValue;
    private Slider _slider;
    private Image _fill;

    private float Percent => (_slider.value - _slider.minValue) / _slider.maxValue;
    
    private void Start()
    {
        _slider = GetComponent<Slider>();
        _fill = _slider.fillRect.GetComponent<Image>();
    }

    private void Update()
    {
        _fill.color = Color.Lerp(minColor, maxColor, colorLerpCurve.Evaluate(Percent));
        _fill.color = Color.Lerp(_fill.color, blinkColor, _blinkValue);

        if (!_blinkStarted && Percent <= blinkStart)
        {
            Blink();
        }
    }

    private void OnDisable()
    {
        this.DOKill();
    }

    private void Blink()
    {
        _blinkStarted = true;

        float duration = blinkFrequencyCurve.Evaluate(Percent) * blinkDuration;
        Vector3 defaultScale = transform.localScale;
        DOTween.To(() => 0f, (x) =>
            {
                _blinkValue = blinkValueCurve.Evaluate(x);
                transform.localScale = defaultScale + (addScale * _blinkValue);
            }, 1f, duration)
            .SetTarget(this)
            .SetEase(Ease.InOutQuad)
            .SetUpdate(UpdateType.Normal, true)
            .OnComplete(() =>
            {
                DOTween.To(() => 0f, (x) => { }, 1f, duration)
                    .SetTarget(this)
                    .SetUpdate(UpdateType.Normal, true)
                    .OnComplete(Blink);
            });
    }
}
