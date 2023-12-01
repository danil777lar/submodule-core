using UnityEngine;

namespace Larje.Core.Tools.EffectsTools
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSystemDestroyer : MonoBehaviour, IEffectDestroyer
    {
        [SerializeField] private float delay;
        [SerializeField] private bool autoDestroy = true;

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
                parts.Stop();
                Destroy(parts.gameObject, parts.main.duration + delay);
            }
        }
    }
}