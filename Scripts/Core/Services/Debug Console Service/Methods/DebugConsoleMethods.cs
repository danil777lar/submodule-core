using System;
using System.Collections.Generic;
using System.Linq;
using Larje.Core;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
        
        [MethodGroup("Sound")]
        public static void SetChannelVolume(string channel, float volume)
        {
            DataService dataService = DIContainer.GetService<DataService>();
            dataService.Data.Settings.SoundData.GetChannel(channel).Volume = volume;
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
        
        #region Level

        [MethodGroup("Level")]
        public static void SpawnLevel()
        {
            ILevelManagerService levelService = DIContainer.GetService<ILevelManagerService>();
            levelService.SpawnCurrentLevel();
        }
        
        [MethodGroup("Level")]
        public static void SetLevelIndex(int index)
        {
            ILevelManagerService levelService = DIContainer.GetService<ILevelManagerService>();
            levelService.SetCurrentLevelIndex(index);
        }
        
        [MethodGroup("Level")]
        public static void StartLevel(LevelStartType startType)
        {
            ILevelManagerService levelService = DIContainer.GetService<ILevelManagerService>();
            levelService.TryStartCurrentLevel(new LevelProcessor.StartData(startType));
        }
        
        [MethodGroup("Level")]
        public static void StopLevel(LevelStopType stopType)
        {
            ILevelManagerService levelService = DIContainer.GetService<ILevelManagerService>();
            levelService.TryStopCurrentLevel(new LevelProcessor.StopData(stopType));
        }
        
        [MethodGroup("Level")]
        public static void SendLevelEvent(string eventName)
        {
            ILevelManagerService levelService = DIContainer.GetService<ILevelManagerService>();
            
            List<Type> derivedTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && t.IsSubclassOf(typeof(LevelEvent))).ToList();
            
            Type type = derivedTypes.FirstOrDefault(x => x.Name.ToLower() == eventName.ToLower());
            if (type != null)
            {
                LevelEvent levelEvent = (LevelEvent) Activator.CreateInstance(type);
                levelService.TrySendEventToCurrentLevel(levelEvent);
            }
        }
        
        #endregion
        
        #region Objects

        [MethodGroup("Objects")]
        public static void CopyCameraPosition(float distanceForward = 5)
        {
            Vector3 point = Camera.main.transform.position + Camera.main.transform.forward * distanceForward;
            ClipboardUtility.CopyToClipboard(point.ToString());
        }
        
        [MethodGroup("Objects")]
        public static void SpawnObject(string key, string position)
        {
            Vector3 pos = Vector3Extensions.Parse(position);
            Addressables.LoadAssetAsync<GameObject>(key).Completed += handle =>
            {
                GameObject prefab = handle.Result;
                GameObject instance = GameObject.Instantiate(prefab);
                instance.transform.position = pos;
                
                GUIDHolder guidHolder = instance.GetComponent<GUIDHolder>();
                if (guidHolder == null)
                {
                    guidHolder = instance.AddComponent<GUIDHolder>();
                }
                guidHolder.GenerateGUID();
                
                Debug.Log($"Object {instance.name} spawned at {pos} with GUID {guidHolder.GUID}");
                ClipboardUtility.CopyToClipboard(guidHolder.GUID);
            };
        }
        
        #endregion
    }
}