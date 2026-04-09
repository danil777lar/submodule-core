using System;
using System.Collections.Generic;
using System.Globalization;

namespace Larje.Core.Services
{
    public partial class GameData
    {
        public IAPServiceData IAPData;
    }
}

[Serializable]
public class IAPServiceData
{
    public List<DiscountEntry> DiscountEntrys = new List<DiscountEntry>();

    public bool HasEntry(string configId)
    {
        return DiscountEntrys.Exists(r => r.ConfigId == configId);
    }

    public bool IsDiscountApplied(string configId)
    {
        DiscountEntry record = DiscountEntrys.Find(r => r.ConfigId == configId);
        return record != null && !record.IsExpired;
    }

    public void ApplyDiscount(string configId)
    {
        ApplyDiscount(configId, TimeSpan.Zero);
    }

    public void ApplyDiscount(string configId, TimeSpan duration)
    {
        DiscountEntry record = DiscountEntrys.Find(r => r.ConfigId == configId);
        if (record != null)
        {
            record.ApplyTime = DateTime.UtcNow.ToString("O");
            record.Duration = duration <= TimeSpan.Zero ? "" : duration.ToString(@"d\.hh\:mm\:ss");
        }
        else
        {
            DiscountEntrys.Add(new DiscountEntry(configId, DateTime.UtcNow, duration));
        }
    }

    public void RemoveDiscount(string configId)
    {
        DiscountEntrys.RemoveAll(r => r.ConfigId == configId);
    }

    public DateTime GetDiscountApplyTime(string configId)
    {
        DiscountEntry record = DiscountEntrys.Find(r => r.ConfigId == configId);
        if (record == null || string.IsNullOrEmpty(record.ApplyTime)) return DateTime.MinValue;
        return DateTime.Parse(record.ApplyTime, null, DateTimeStyles.RoundtripKind);
    }

    public DateTime GetDiscountExpiryTime(string configId)
    {
        DiscountEntry record = DiscountEntrys.Find(r => r.ConfigId == configId);
        if (record == null || record.IsTimeless) return DateTime.MaxValue;
        return GetDiscountApplyTime(configId) + TimeSpan.Parse(record.Duration);
    }
}

[Serializable]
public class DiscountEntry
{
    public string ConfigId;
    public string ApplyTime;
    public string Duration;

    public bool IsTimeless => string.IsNullOrEmpty(Duration);

    public bool IsExpired
    {
        get
        {
            if (IsTimeless) return false;
            DateTime applyTime = DateTime.Parse(ApplyTime, null, DateTimeStyles.RoundtripKind);
            return DateTime.UtcNow > applyTime + TimeSpan.Parse(Duration);
        }
    }

    public DiscountEntry(string configId, DateTime applyTime, TimeSpan duration = default)
    {
        ConfigId = configId;
        ApplyTime = applyTime.ToString("O");
        Duration = duration <= TimeSpan.Zero ? "" : duration.ToString(@"d\.hh\:mm\:ss");
    }
}
