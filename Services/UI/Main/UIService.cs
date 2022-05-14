using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Larje.Core.Services
{
    [BindService(typeof(UIService))]
    public class UIService : Service
    {
        [SerializeField] private Transform _screenHolder;
        [SerializeField] private Transform _popupHolder;
        [Space]
        [SerializeField] private string _defaultScreenId;


        private void Awake()
        {
            ShowScreen(_defaultScreenId, false);
        }


        public async void ShowScreen(string id, bool withAnim = true) 
        {
            foreach (UIScreen oldScreen in _screenHolder.GetComponentsInChildren<UIScreen>())
                oldScreen.Close();

            var op = Addressables.InstantiateAsync($"Screen/{id}", _screenHolder.transform);
            await op.Task;
            if (op.IsDone && withAnim)
            {
                op.Result.gameObject.GetComponent<UIScreen>().Open();
            }
        }


        public override void Init(){}
    }
}