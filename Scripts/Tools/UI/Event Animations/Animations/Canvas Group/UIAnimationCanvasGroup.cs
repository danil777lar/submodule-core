using DG.Tweening;
using UnityEngine;

public class UIAnimationCanvasGroup : UIAnimationBase
{
    [SerializeField] private float duration = 0.25f;
    [Space]
    [SerializeField] private float valueFrom = 0f;
    [SerializeField] private float valueTo = 1f;
    [SerializeField] private bool setToFromOnAwake = true;
    
    private CanvasGroup _canvasGroup;
    
    protected override float OnEvent(bool forward)
    {
        this.DOKill();
        
        _canvasGroup.DOFade(forward ? valueTo : valueFrom, duration)
            .SetUpdate(true)
            .SetTarget(this);
        
        return duration;
    }

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (setToFromOnAwake)
        {
            _canvasGroup.alpha = valueFrom;
        }
    }
}
