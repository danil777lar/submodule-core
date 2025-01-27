using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringExtensions
{
    public static string LerpLinear(this string from, string to, float t)
    {
        int symbolsCount = GetCharArrays(from, to, out char[] fromAr, out char[] toAr);
        int currentSymbol = EvaluateInt(0, symbolsCount, t);

        char[] output = new char[symbolsCount];
        for (int i = 0; i < symbolsCount; i++)
        {
            output[i] = i <= currentSymbol ? toAr[i] : fromAr[i];
        }

        return new string(output);
    }
    
    private static int GetCharArrays(string from, string to, out char[] fromArray, out char[] toArray)
    {
        int symbolsCount = Mathf.Max(from.Length, to.Length);
        fromArray = new char[symbolsCount];
        toArray = new char[symbolsCount];

        for (int i = 0; i < symbolsCount; i++)
        {
            fromArray[i] = i < from.Length ? from[i] : ' ';
            toArray[i] = i < to.Length ? to[i] : ' ';
        }
        
        return symbolsCount;
    }

    private static int EvaluateInt(int from, int to, float t)
    {
        return Mathf.FloorToInt(Mathf.Lerp(from, to, t));
    }
}
