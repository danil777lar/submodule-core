using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services
{
    [Serializable]
    public class GTableLocalization
    {
        public string Key;
        public List<GTableLocalizationValue> Values;
    }

    [Serializable]
    public class GTableLocalizationValue
    {
        public string LanguageCode;
        public string Value;
    }
}