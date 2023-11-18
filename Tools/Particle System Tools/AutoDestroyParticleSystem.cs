using UnityEngine;

namespace Larje.Core.Tools.ParticleSystemTools
{
    [RequireComponent(typeof(ParticleSystem))]
    public class AutoDestroyParticleSystem : MonoBehaviour
    {
        [SerializeField] private float delay;
        
        private void Start()
        {
            ParticleSystem parts = GetComponent<ParticleSystem>();
            Destroy(parts.gameObject, parts.main.duration + delay);
        }
    }
}