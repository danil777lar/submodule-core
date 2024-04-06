using System.Collections;
using UnityEngine;

namespace Larje.Core.Tools.EffectsTools
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSystemDestroyer : MonoBehaviour, IEffectDestroyer
    {
        [SerializeField] private bool stopOnDestroy = false;
        [SerializeField] private bool autoDestroy = true;
        [Space]
        [SerializeField] private float delay;
        [SerializeField] private GameObject destroyTargetOverride;
 
        private bool _isDestroying;
        
        private void Start()
        {
            if (autoDestroy)
            {
                DestroyEffect();
            }
        }

        public void DestroyEffect()
        {
            if (!_isDestroying)
            {
                _isDestroying = true;
                
                ParticleSystem parts = GetComponent<ParticleSystem>();
                if (stopOnDestroy)
                {
                    parts.Stop();
                }
                
                GameObject target = destroyTargetOverride ? destroyTargetOverride : gameObject;
                Destroy(target, parts.main.duration + delay);
            }
        }
    }
}