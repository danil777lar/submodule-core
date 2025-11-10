using DG.Tweening;
using UnityEngine;

public class UIAnimationTransformRotation : UIAnimationBase
{
    [Header("Space")]
    [SerializeField] private Vector3 offsetFrom = Vector3.forward * 90f;
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
        transform.DOLocalRotate(forward ? _valueTo : _valueFrom, duration)
            .SetEase(ease)
            .SetUpdate(true)
            .SetTarget(this);
        
        return duration;
    }

    private void Awake()
    {
        _valueFrom = transform.localEulerAngles + offsetFrom;
        _valueTo = transform.localEulerAngles + offsetTo;
        
        if (setToFromOnAwake)
        {
            transform.localRotation = Quaternion.Euler(_valueFrom);
        }
    }
}
