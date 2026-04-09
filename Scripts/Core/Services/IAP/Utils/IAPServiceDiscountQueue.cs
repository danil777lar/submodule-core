using System;
using System.Collections.Generic;
using Larje.Core;
using Larje.Core.Services;
using UnityEngine;

public class IAPServiceDiscountQueue : MonoBehaviour
{
    [SerializeField] private float checkInterval = 5f;
    [SerializeField] private float discountDuration = 60f;
    [SerializeField] private List<IAPProductConfig> queue;

    [InjectService] private IIAPService _iapService;
    [InjectService] private IDataService _dataService;

    private float _timeSinceLastCheck = 0f;

    private void Start()
    {
        DIContainer.InjectTo(this);
        TryAdvanceDiscount();
    }

    private void Update()
    {
        _timeSinceLastCheck += Time.deltaTime;
        if (_timeSinceLastCheck >= checkInterval)
        {            
            _timeSinceLastCheck = 0f;
            TryAdvanceDiscount();
        }
    }

    private void TryAdvanceDiscount()
    {
        if (queue.Count == 0)
        {
            return;
        }

        for (int i = 0; i < queue.Count; i++)
        {
            if (_iapService.IsDiscountApplied(queue[i]))
            {
                return;
            }
        }

        int nextIndex = -1;
        long latestTicks = long.MinValue;
        IAPServiceData iapData = _dataService.GameData.IAPData;

        for (int i = 0; i < queue.Count; i++)
        {
            if (iapData.HasEntry(queue[i].Id))
            {
                long ticks = iapData.GetDiscountApplyTime(queue[i].Id).Ticks;
                if (ticks > latestTicks)
                {
                    latestTicks = ticks;
                    nextIndex = (i + 1) % queue.Count;
                }
            }
        }

        if (nextIndex < 0)
        {
            nextIndex = 0;
        }

        for (int i = 0; i < queue.Count; i++)
        {
            int candidate = (nextIndex + i) % queue.Count;
            if (!_iapService.IsProductPurchased(queue[candidate]))
            {
                _iapService.ApplyDiscount(queue[candidate], TimeSpan.FromMinutes(discountDuration));
                return;
            }
        }
    }
}
