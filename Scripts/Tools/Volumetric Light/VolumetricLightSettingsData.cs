using UnityEngine;

namespace Larje.Core.Services
{
    public partial class GraphicsSettings
    {
        public VolumetricLightQuality VolumetricLightQuality = VolumetricLightQuality.Medium;
    }
}

public enum VolumetricLightQuality
{
    Low,
    Medium,
    High
}
