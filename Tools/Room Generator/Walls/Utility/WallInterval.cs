using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallInterval
{
    public readonly double FromPercent;
    public readonly double ToPercent;
    public readonly double FromHeight;
    public readonly double ToHeight;
            
    public WallInterval(double fromPercent, double toPercent, double fromHeight, double toHeight)
    {
        FromPercent = fromPercent;
        ToPercent = toPercent;
        FromHeight = fromHeight;
        ToHeight = toHeight;
    }

    public bool Contains(double percent, double height)
    {
        return ContainsPercent(percent) && ContainsHeight(height);
    }

    private bool ContainsPercent(double percent)
    {
        if (FromPercent <= ToPercent)
        {
            return percent > FromPercent && percent < ToPercent;
        }
        else
        {
            return percent > FromPercent || percent < ToPercent;
        }
    }
            
    private bool ContainsHeight(double percent)
    {
        if (FromHeight <= ToHeight)
        {
            return percent > FromHeight && percent < ToHeight;
        }
        else
        {
            return percent > FromHeight || percent < ToHeight;
        }
    }
}
