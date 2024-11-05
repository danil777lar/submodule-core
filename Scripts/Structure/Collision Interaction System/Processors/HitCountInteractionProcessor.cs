using System;

namespace Larje.Core.Tools.Interaction.Processors
{
    public class HitCountInteractionProcessor : InteractionProcessor
    {
        private readonly int _hitsToKill;
        private int _currentHitCount;

        public HitCountInteractionProcessor(int hitsToKill, Action interacted, Action completed) : base(interacted, completed)
        {
            _hitsToKill = hitsToKill;
            _currentHitCount = 0;
        }

        public override void ProcessInteraction(InteractionData data)
        {
            if (_currentHitCount >= _hitsToKill)
            {
                return;
            }

            _currentHitCount++;
            if (_currentHitCount < _hitsToKill)
            {
                _interacted?.Invoke();
            }
            else
            {
                _completed?.Invoke();
            }
        }

        public override float GetProgress()
        {
            return (float)_currentHitCount / (float)_hitsToKill;
        }
    }
}