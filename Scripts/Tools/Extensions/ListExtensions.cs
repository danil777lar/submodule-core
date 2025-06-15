using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Tools
{
    public static class ListExtensions
    {
        public static void Swap<T>(this IList<T> list, int i, int j)
        {
            (list[i], list[j]) = (list[j], list[i]);
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list.Swap(i, Random.Range(i, list.Count));
            }
        }
    }
}