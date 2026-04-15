using System;
using UnityEngine;

namespace Larje.Core.Services
{
    [BindService(typeof(IAdsService))]
    public class PlaceholderAdsService : Service, IAdsService
    {
        public bool Initialized => true;

        public bool InterstitialAdAvailable => false;

        public bool RewardedAdAvailable => false;

        public bool BannerShowing => false;

        public float BannerHeight => 0f;

        public event Action EventBannerShown;
        public event Action EventBannerHidden;

        public override void Init()
        {
        }

        public bool ShowAppOpenAd()
        {
            return true;
        }

        public void ShowInterstitial(int interIndex = 0)
        {
        }

        public void ShowRewarded(Action onAdShowStart, Action onAdShowClick, Action onAdShowComplete, Action onAdShowFailed)
        {
            onAdShowFailed?.Invoke();
        }

        public void SetActiveNoAdsMode(bool noAdsActive)
        {
        }
    }
}
