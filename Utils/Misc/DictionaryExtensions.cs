using System.Collections.Generic;

namespace Hermit
{
    public static class DictionaryExtensions
    {
        public static void AddToList<TKey, TValue>(this Dictionary<TKey, List<TValue>> dict, TKey key, TValue value)
        {
            if (dict.TryGetValue(key, out var values)) { values.Add(value); }
            else { dict.Add(key, new List<TValue> {value}); }
        }
    }
}