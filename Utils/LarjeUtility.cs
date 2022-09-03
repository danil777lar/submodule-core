using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Utility
{
    public static class LarjeUtility
    {
        public static bool CastCollider2D(Collider2D collider, Vector2 direction, float distance) 
        {
            RaycastHit2D[] results = new RaycastHit2D[1];
            return collider.Cast(direction, results, distance) > 0;
        }

        public static Vector2Int FloorVector2(Vector2 vector) 
        {
            Vector2Int result = Vector2Int.zero;
            result.x = Mathf.FloorToInt(vector.x);
            result.y = Mathf.FloorToInt(vector.y);
            return result;
        }

        public static Color SetColorAlpha(Color color, float alphaValue) 
        {
            return new Color(color.a, color.g, color.b, alphaValue);
        }

        public static Color SetColorSaturation(Color color, float saturationValue) 
        {
            float grayValue = (color.a + color.g + color.b) / 3f;
            return Color.LerpUnclamped(Color.white * grayValue, color, saturationValue);
        }
    }
}
