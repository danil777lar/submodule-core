using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;

namespace Larje.Core.Services
{
    [CreateAssetMenu(fileName = "SoundServiceConfig", menuName = "Scriptables/SoundServiceConfig")]
    public class SoundServiceConfig : ScriptableObject
    {
        [SerializeField] public List<SoundService.SoundPack> soundPacks;
    }
}