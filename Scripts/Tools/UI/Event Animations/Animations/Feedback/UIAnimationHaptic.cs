using UnityEngine;
using ProjectConstants;
using Larje.Core.Services;
using Larje.Core;

public class UIAnimationHaptic : UIAnimationBase
{
    [SerializeField] private HapticType hapticType;

    [InjectService] private IHapticService _hapticService;
    
    protected override float OnEvent(bool forward)
    {
        _hapticService?.PlayHaptic(hapticType);  
        return 0f;
    }

    private void Start()
    {
        DIContainer.InjectTo(this);
    }
}
