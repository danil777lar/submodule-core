using UnityEngine;

namespace Larje.Core.Tools.Interaction.Sources
{
    public class TrapInteractionSource : InteractionSource
    {
        [SerializeField] private int damage;
        
        protected override InteractionData GenerateData()
        {
            return new InteractionData(damage);
        }
    }
}