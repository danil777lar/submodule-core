using System;
using System.Collections.Generic;

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
            record.ApplyTimeTicks = DateTime.UtcNow.Ticks;
            record.DurationTicks = duration.Ticks;
        }
        else
        {
            DiscountEntrys.Add(new DiscountEntry(configId, DateTime.UtcNow.Ticks, duration.Ticks));
        }
    }

    public void RemoveDiscount(string configId)
    {
        DiscountEntrys.RemoveAll(r => r.ConfigId == configId);
    }

    public DateTime GetDiscountApplyTime(string configId)
    {
        DiscountEntry record = DiscountEntrys.Find(r => r.ConfigId == configId);
        return record != null ? new DateTime(record.ApplyTimeTicks, DateTimeKind.Utc) : DateTime.MinValue;
    }

    public DateTime GetDiscountExpiryTime(string configId)
    {
        DiscountEntry record = DiscountEntrys.Find(r => r.ConfigId == configId);
        if (record == null || record.IsTimeless) return DateTime.MaxValue;
        return new DateTime(record.ApplyTimeTicks + record.DurationTicks, DateTimeKind.Utc);
    }
}

[Serializable]
public class DiscountEntry
{
    public string ConfigId;
    public long ApplyTimeTicks;
    public long DurationTicks;

    public bool IsTimeless => DurationTicks <= 0;
    public bool IsExpired => !IsTimeless && DateTime.UtcNow.Ticks > ApplyTimeTicks + DurationTicks;

    public DiscountEntry(string configId, long applyTimeTicks, long durationTicks = 0)
    {
        ConfigId = configId;
        ApplyTimeTicks = applyTimeTicks;
        DurationTicks = durationTicks;
    }
}
