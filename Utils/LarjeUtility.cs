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
    }
}
