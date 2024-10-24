using System;

namespace Larje.Core.Tools.Interaction.Processors
{
    public class HealthInteractionProcessor : InteractionProcessor
    {
        private readonly int _fullHealth;
        private int _currentHealth;

        public HealthInteractionProcessor(int fullHealth, Action interacted, Action completed) : base(interacted, completed)
        {
            _fullHealth = fullHealth;
            _currentHealth = fullHealth;
        }

        public override void ProcessInteraction(InteractionData data)
        {
            if (_currentHealth <= 0)
            {
                return;
            }

            _currentHealth -= data.points;
            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                _completed?.Invoke();
            }
            else
            {
                _interacted?.Invoke();
            }
        }

        public override float GetProgress()
        {
            if (_currentHealth == 0)
            {
                return 1f;
            }
            else
            {
                return 1f - ((float)_currentHealth / (float)_fullHealth);   
            }
        }
    }
}