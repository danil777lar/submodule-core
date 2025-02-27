using NUnit.Framework.Internal;
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
}
