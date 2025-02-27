using NUnit.Framework.Internal;
using ProjectConstants;
using UnityEngine;

public static partial class DebugConsoleMethods
{
    public static void Test(string testTextA, string testTextB)
    {
        Debug.Log("Test: " + testTextA + " " + testTextB);
    }
    
    public static void Test(int testInt, float testFloat)
    {
        Debug.Log("Test: " + testInt + " " + testFloat);
    }
    
    public static void Test(CurrencyCounter counter)
    {
        
    }
    
    public static void Test(CurrencyType currencyType, CurrencyPlacementType placementType, int count)
    {
        Debug.Log($"Test: {currencyType} {placementType} {count}");   
    }
}
