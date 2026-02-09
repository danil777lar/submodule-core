using UnityEngine;
using ProjectConstants;

namespace Larje.Core.Services
{
    public interface IHapticService
    {
        public void PlayHaptic(HapticType hapticType);
        public void PlayHaptic(HapticParameters hapticParameters);
    }
}
