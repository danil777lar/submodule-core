using System;

namespace Larje.Core.Tools.Interaction
{
    public abstract class InteractionProcessor
    {
        protected Action _interacted;
        protected Action _completed;

        public InteractionProcessor(Action interacted, Action completed)
        {
            _interacted = interacted;
            _completed = completed;
        }
        
        public abstract void ProcessInteraction(InteractionData data);
        public abstract float GetProgress();
    }
}