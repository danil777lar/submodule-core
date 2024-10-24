using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Larje.Core.Tools.EffectsTools;
using UnityEngine;

namespace Larje.Core.Tools.EffectsTools
{
    [RequireComponent(typeof(TrailRenderer))]
    public class TrailDestroyer : MonoBehaviour, IEffectDestroyer
    {
        [SerializeField] private float destroyDuration = 0.25f;
        [SerializeField] private float destroyDelay = 0f;
        [SerializeField] private bool autoDestroy = true;

        private bool _isDestroying;
        
        public void DestroyEffect()
        {
            if (!_isDestroying)
            {
                _isDestroying = true;
                StartCoroutine(DestroyCoroutine());
            }
        }

        private void Start()
        {
            if (autoDestroy)
            {
                DestroyEffect();
            }
        }

        private IEnumerator DestroyCoroutine()
        {
            yield return new WaitForSeconds(destroyDelay);

            TrailRenderer trail = GetComponent<TrailRenderer>();
            trail.DOTime(0f, destroyDuration)
                .OnComplete(() => Destroy(gameObject));
        }
    }
}