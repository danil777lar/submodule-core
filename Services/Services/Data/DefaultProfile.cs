using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Larje.Core.Services
{
    [CreateAssetMenu(fileName = "DefaultProfile", menuName = "Scriptables/DefaultDataProfile")]
    public class DefaultProfile : ScriptableObject
    {
        public GameData profileData;
    }
}