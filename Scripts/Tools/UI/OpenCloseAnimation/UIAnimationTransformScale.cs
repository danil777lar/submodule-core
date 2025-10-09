using System;
using DG.Tweening;
using UnityEngine;

public class UIAnimationTransformScale : UIAnimationBase
{
    [Header("Space")]
    [SerializeField] private Vector3 valueFrom = Vector3.one * 0.5f;
    [SerializeField] private Vector3 valueTo = Vector3.one;
    [Space]
    [SerializeField] private float durationForward = 0.25f;
    [SerializeField] private Ease easeForward = Ease.OutBack;
    [Space]
    [SerializeField] private float durationBackwards = 0.25f;
    [SerializeField] private Ease easeBackwards = Ease.InBack;
    [Space]
    [SerializeField] private bool setToFromOnAwake = true;
    
    protected override float OnEvent(bool forward)
    {
        this.DOKill();
        
        float duration = forward ? durationForward : durationBackwards;
        Ease ease = forward ? easeForward : easeBackwards;
        transform.DOScale(forward ? valueTo : valueFrom, duration)
            .SetEase(ease)
            .SetUpdate(true)
            .SetTarget(this);
        
        return duration;
    }

    private void Awake()
    {
        if (setToFromOnAwake)
        {
            transform.localScale = valueFrom;
        }
    }
}
