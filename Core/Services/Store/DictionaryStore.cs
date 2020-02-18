using System;
using System.Collections.Generic;

namespace Hermit
{
    public class DictionaryStore : Singleton<DictionaryStore>, IStore, IDisposable
    {
        private readonly Dictionary<string, object> _cache = new Dictionary<string, object>();

        public string StoreId { get; private set; }

        public List<string> Keys { get; } = new List<string>();

        public void SetStoreId(string id)
        {
            StoreId = id;
        }

        public bool Has(string key)
        {
            return _cache.ContainsKey(key);
        }

        public bool Is<T>(string key)
        {
            if (!Has(key)) { return false; }

            if (_cache.TryGetValue(key, out var ret)) { return ret is T; }

            return false;
        }

        public T Get<T>(string key)
        {
            if (_cache.TryGetValue(key, out var ret)) { return (T) ret; }

            return default;
        }

        public T Set<T>(string key, T data)
        {
            if (!Keys.Contains(key)) { Keys.Add(key); }

            _cache[key] = data;

            return data;
        }

        public bool Is(string key, Type type)
        {
            if (!Has(key)) { return false; }

            if (_cache.TryGetValue(key, out var ret)) { return ret.GetType() == type; }

            return false;
        }

        public object Get(string key)
        {
            return _cache.TryGetValue(key, out var ret) ? ret : null;
        }

        public object Set(string key, object obj)
        {
            if (!Keys.Contains(key)) { Keys.Add(key); }

            _cache[key] = obj;
            return obj;
        }

        public void Remove(string key)
        {
            if (_cache.ContainsKey(key)) { _cache.Remove(key); }

            if (Keys.Contains(key)) { Keys.Remove(key); }
        }

        public void ClearAll()
        {
            _cache.Clear();
            Keys.Clear();
        }

        public void Dispose()
        {
            ClearAll();
        }
    }
}