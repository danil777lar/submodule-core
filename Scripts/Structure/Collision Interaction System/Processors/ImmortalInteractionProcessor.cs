using System;

namespace Larje.Core.Tools.Interaction.Processors
{
    public class ImmortalInteractionProcessor : InteractionProcessor
    {
        public ImmortalInteractionProcessor(Action interacted, Action completed) : base(interacted, completed)
        {
        }

        public override void ProcessInteraction(InteractionData data)
        {
            _interacted?.Invoke();
        }

        public override float GetProgress()
        {
            return 0f;
        }
    }
}