using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using UnityEngine;

public class BannerBack : MonoBehaviour
{
    [InjectService] private IAdsService _adsService;
    
    private RectTransform _rect;
    
    private void Start()
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);

        _rect = transform as RectTransform;
        
        if (_adsService.BannerShowing)
        {
            Show();
        }
        _adsService.EventBannerShown += Show;
        _adsService.EventBannerHidden += Hide;
    }

    private void OnDestroy()
    {
        _adsService.EventBannerShown -= Show;
        _adsService.EventBannerHidden -= Hide;
    }

    private void Show()
    {
        _rect.sizeDelta = new Vector2(_rect.sizeDelta.x, _adsService.BannerHeight);
    }
    
    private void Hide()
    {
        _rect.sizeDelta = new Vector2(_rect.sizeDelta.x, 0f);
    } 
}
