using System;
using ProjectConstants;

namespace Larje.Core.Tools.Interaction
{
    public interface IInteractionTarget
    {
        public event Action Interacted;
        public event Action Completed;
        public void TryInteract(InteractionData data);
        public float GetInteractionProgress();
        public InteractionTargetType GetTargetType();
    }
}