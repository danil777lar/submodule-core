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
    
    private Vector3 _valueFrom;
    private Vector3 _valueTo;
    
    protected override float OnEvent(bool forward)
    {
        this.DOKill();
        
        float duration = forward ? durationForward : durationBackwards;
        Ease ease = forward ? easeForward : easeBackwards;
        transform.DOLocalMove(forward ? _valueTo : _valueFrom, duration)
            .SetEase(ease)
            .SetUpdate(true)
            .SetTarget(this);
        
        return duration;
    }

    private void Awake()
    {
        _valueFrom = transform.localPosition + offsetFrom;
        _valueTo = transform.localPosition + offsetTo;
        
        if (setToFromOnAwake)
        {
            transform.localPosition = _valueFrom;
        }
    }
}
