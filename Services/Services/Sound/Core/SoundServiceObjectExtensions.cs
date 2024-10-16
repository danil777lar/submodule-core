using Larje.Core.Services;
using UnityEngine;

public static class SoundServiceObjectExtensions
{
    public static void SoundServiceStop(this object target)
    {
        if (ServiceLocator.Instance != null)
        {
            ServiceLocator.Instance.GetService<SoundService>().Stop(target);
        }
    }
}
