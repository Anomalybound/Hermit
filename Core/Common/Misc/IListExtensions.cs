using System;
using System.Collections.Generic;

namespace Hermit.Common
{
    public static class IListExtensions
    {
        public static IList<T> Shuffle<T>(this IList<T> source, int? seed = null)
        {
            var random = new Random(seed ?? DateTime.Now.Millisecond);
            for (var i = 0; i < source.Count - 1; i++)
            {
                var randomIndex = random.Next(i, source.Count);
                var temp = source[randomIndex];
                source[randomIndex] = source[i];
                source[i] = temp;
            }

            return source;
        }
    }
}