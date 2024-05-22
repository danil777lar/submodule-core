using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using UnityEngine;

public class BannerOffset : MonoBehaviour
{
    [InjectService] private IAdsService _adsService;

    private float _defaultOffset;
    private RectTransform _rect;
    
    private void Awake()
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);

        _rect = transform as RectTransform;
        _defaultOffset = _rect.offsetMin.y;
        
        if (_adsService.BannerShowing)
        {
            ShowOffset();
        }
        _adsService.EventBannerShown += ShowOffset;
        _adsService.EventBannerHidden += HideOffset;
    }

    private void OnDestroy()
    {
        _adsService.EventBannerShown -= ShowOffset;
        _adsService.EventBannerHidden -= HideOffset;
    }

    private void ShowOffset()
    {
        _rect.offsetMin = new Vector2(_rect.offsetMin.x, _defaultOffset + _adsService.BannerHeight);
    }
    
    private void HideOffset()
    {
        _rect.offsetMin = new Vector2(_rect.offsetMin.x, _defaultOffset);
    } 
}
