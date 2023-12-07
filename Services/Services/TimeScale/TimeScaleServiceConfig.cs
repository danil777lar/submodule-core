using UnityEngine;

namespace Larje.Core.Services
{
    [CreateAssetMenu(menuName = "Configs/Time Scale Config", fileName = "Time Scale Config")]
    public class TimeScaleServiceConfig : ScriptableObject
    {
        [field: SerializeField] public TimeScaleAnimation[] TimeScaleAnimations { get; private set; }
    }
}