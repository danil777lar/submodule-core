using ProjectConstants;
using UnityEngine;

namespace Larje.Core.Tools.Interaction
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class InteractionSource : MonoBehaviour
    {
        [SerializeField] private InteractionTargetTypes targetTypes;
        
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.rigidbody != null)
            {
                TryInteract(collision.rigidbody);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.attachedRigidbody != null)
            {
                TryInteract(other.attachedRigidbody);
            }
        }

        private void TryInteract(Rigidbody targetRigidbody)
        {
            if (targetRigidbody.TryGetComponent(out IInteractionTarget target))
            {
                if (targetTypes.HasFlag((InteractionTargetTypes)target.GetTargetType()))
                {
                    target.TryInteract(GenerateData());
                }
            }
        }

        protected abstract InteractionData GenerateData();
    }
}