using DG.Tweening;
using UnityEngine;

public class UIAnimationTransformPosition : UIAnimationBase
{
    [Header("Space")]
    [SerializeField] private Vector3 offsetFrom = Vector3.up * -200f;
    [SerializeField] private Vector3 offsetTo = Vector3.zero;
    [Space]
    [SerializeField] private float durationForward = 0.25f;
    [SerializeField] private Ease easeForward = Ease.OutBack;
    [Space]
    [SerializeField] private float durationBackwards = 0.25f;
    [SerializeField] private Ease easeBackwards = Ease.InBack;
    [Space]
    [SerializeField] private bool setToFromOnAwake = true;
    
    private Vector2 _valueFrom;
    private Vector2 _valueTo;
    
    protected override float OnEvent(bool forward)
    {
        this.DOKill();
        
        RectTransform rectTransform = transform as RectTransform;

        float duration = forward ? durationForward : durationBackwards;
        Ease ease = forward ? easeForward : easeBackwards;
        rectTransform.DOAnchorPos(forward ? _valueTo : _valueFrom, duration)
            .SetEase(ease)
            .SetUpdate(true)
            .SetTarget(this);
        
        return duration;
    }

    private void Awake()
    {
        RectTransform rectTransform = transform as RectTransform;

        _valueFrom = rectTransform.anchoredPosition + (Vector2)offsetFrom;
        _valueTo = rectTransform.anchoredPosition + (Vector2)offsetTo;
        
        if (setToFromOnAwake)
        {
            rectTransform.anchoredPosition = _valueFrom;
        }
    }
}
