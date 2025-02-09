using System;

namespace Larje.Core.Services
{
    public interface IAdsService
    {
        public bool Initialized { get; }
        public bool InterstitialAdAvailable { get; }
        public bool RewardedAdAvailable { get; }
        public bool BannerShowing { get; }
        public float BannerHeight { get; }

        public event Action EventBannerShown;
        public event Action EventBannerHidden;
        
        public void ShowInterstitial(int interIndex = 0);
        
        public void ShowRewarded(Action onAdShowStart, Action onAdShowClick, Action onAdShowComplete, Action onAdShowFailed);
    }
}