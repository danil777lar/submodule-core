using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Larje.Core.Services.UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonClickAnimation : MonoBehaviour
    {
        [SerializeField] private float _scaleValue = 0.8f;
        [SerializeField] private float _inAnimDuration = 0.1f;
        [SerializeField] private float _outAnimDuration = 0.25f;
        [SerializeField] private Ease _inAnimEase = Ease.Linear;
        [SerializeField] private Ease _outAnimEase = Ease.OutBack;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnButtonCLlicked);            
        }

        private void OnButtonCLlicked() 
        {
            this.DOKill();
            transform.DOScale(_scaleValue, _inAnimDuration)
                .SetTarget(this)
                .SetEase(_inAnimEase)
                .OnComplete(() => 
                {
                    transform.DOScale(1f, _outAnimDuration)
                    .SetTarget(this)
                    .SetEase(_outAnimEase);
                });
        }
    }
}
