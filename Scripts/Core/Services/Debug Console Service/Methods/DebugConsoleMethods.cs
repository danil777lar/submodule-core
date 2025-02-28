using Larje.Core;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using NUnit.Framework.Internal;
using ProjectConstants;
using UnityEngine;

namespace Larje.Core.Services.DebugConsole
{
    public static partial class DebugConsoleMethods
    {
        #region Currency

        [MethodGroup("Currency")]
        public static void AddCurrency(CurrencyType type, CurrencyPlacementType place, int count)
        {
            ICurrencyService currencyService = DIContainer.GetService<ICurrencyService>();
            currencyService.AddCurrency(type, place, count);
        }

        [MethodGroup("Currency")]
        public static void SpendCurrency(CurrencyType type, CurrencyPlacementType place, int count)
        {
            ICurrencyService currencyService = DIContainer.GetService<ICurrencyService>();
            currencyService.TrySpendCurrency(type, place, count);
        }

        [MethodGroup("Currency")]
        public static void SetCurrency(CurrencyType type, CurrencyPlacementType place, int count)
        {
            ICurrencyService currencyService = DIContainer.GetService<ICurrencyService>();
            currencyService.SetCurrency(type, place, count);
        }

        #endregion

        #region Sound

        [MethodGroup("Sound")]
        public static void PlaySound(SoundType sound)
        {
            SoundService soundService = DIContainer.GetService<SoundService>();
            soundService.Play(sound);
        }

        #endregion

        #region UI

        [MethodGroup("UI")]
        public static void OpenScreen(UIScreenType screen)
        {
            UIService uiService = DIContainer.GetService<UIService>();
            uiService.GetProcessor<UIScreenProcessor>().OpenScreen(new UIScreen.Args(screen));
        }

        [MethodGroup("UI")]
        public static void OpenPopup(UIPopupType popup, UIPopupCombinationType combination)
        {
            UIService uiService = DIContainer.GetService<UIService>();
            uiService.GetProcessor<UIPopupProcessor>().OpenPopup(new UIPopup.Args(popup, combination));
        }

        [MethodGroup("UI")]
        public static void OpenToast(UIToastType toast, string text)
        {
            UIService uiService = DIContainer.GetService<UIService>();
            uiService.GetProcessor<UIToastProcessor>().OpenToast(new UIToast.Args(toast, text));
        }

        #endregion
    }
}