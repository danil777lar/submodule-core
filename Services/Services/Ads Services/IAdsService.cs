using System;

namespace Larje.Core.Services
{
    public interface IAdsService
    {
        public bool Initialized { get; }
        public bool InterstitialAdAvailable { get; }
        public bool RewardedAdAvailable { get; }

        public void ShowInterstitial();
        
        public void ShowRewarded(Action onAdShowStart, Action onAdShowClick, Action onAdShowComplete, Action onAdShowFailed);
    }
}