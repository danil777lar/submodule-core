using Larje.Core;
using Larje.Core.Services;
using UnityEngine;

public static class SoundServiceObjectExtensions
{
    public static void SoundServiceStop(this object target)
    {
        if (DIContainer.IsInitialized())
        {
            DIContainer.GetService<SoundService>().Stop(target);
        }
    }
}
