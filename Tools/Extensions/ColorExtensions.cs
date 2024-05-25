using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExtensions
{
    public static Color SetAlpha(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }
    
    public static Color SetSaturation(this Color color, float saturationValue) 
    {
        float grayValue = (color.a + color.g + color.b) / 3f;
        return Color.LerpUnclamped(Color.white * grayValue, color, saturationValue);
    }
}
