using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services
{
    [CreateAssetMenu(fileName = "SoundServiceConfig", menuName = "Scriptables/SoundServiceConfig")]
    public class SoundServiceConfig : ScriptableObject
    {
        public List<SoundService.SoundPack> soundPacks;

    }
}