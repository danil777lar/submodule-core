using UnityEngine;

namespace Larje.Core.Services
{
    public interface ILocalizationService
    {
        public bool LocalizationLoaded { get; }

        public string GetLocalizationValue(string key);
        public float GetLoadingPercent();
    }
}