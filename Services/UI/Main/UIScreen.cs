using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class UIScreen : MonoBehaviour
{
    [SerializeField] private float _animDuration = 0f;
    [SerializeField] private Ease _animEase = Ease.Linear;
    [Space]
    [SerializeField] private float _fadeValue = 1f;
    [SerializeField] private Vector3 _scaleValue = Vector3.one;
    [SerializeField] private Vector3 _positionValue = Vector3.zero;
    


    private CanvasGroup _canvasGroup;


    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }


    public void Open() 
    {
        _canvasGroup.alpha = _fadeValue;
        transform.localScale = _scaleValue;
        transform.localPosition = _positionValue;

        this.DOKill();
        _canvasGroup.DOFade(1f, _animDuration).SetTarget(this);
        transform.DOScale(1f, _animDuration).SetEase(_animEase).SetTarget(this);
        transform.DOLocalMove(Vector2.zero, _animDuration).SetEase(_animEase).SetTarget(this);
    }

    public void Close() 
    {
        this.DOKill();
        _canvasGroup.DOFade(_fadeValue, _animDuration).SetTarget(this);
        transform.DOScale(_scaleValue, _animDuration).SetEase(_animEase).SetTarget(this);
        transform.DOLocalMove(Vector2.one * _positionValue, _animDuration).SetEase(_animEase).SetTarget(this)
            .OnComplete(() => Destroy(gameObject));
    }
}
