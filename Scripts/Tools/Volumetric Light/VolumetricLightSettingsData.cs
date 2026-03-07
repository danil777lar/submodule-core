using UnityEngine;

namespace Larje.Core.Services
{
    public partial class GraphicsSettings
    {
        public VolumetricLightQuality VolumetricLightQuality = VolumetricLightQuality.High;
    }
}

public enum VolumetricLightQuality
{
    Low = 0,
    Medium = 1,
    High = 2
}
