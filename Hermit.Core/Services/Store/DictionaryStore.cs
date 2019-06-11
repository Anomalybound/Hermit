using System;
using System.Collections.Generic;

namespace Hermit
{
    public class DictionaryStore : IStore
    {
        private readonly Dictionary<string, object> _cache = new Dictionary<string, object>();

        public string StoreId { get; private set; }

        public void SetStoreId(string id)
        {
            StoreId = id;
        }

        public bool Has(string id)
        {
            return _cache.ContainsKey(id);
        }

        public bool Is<T>(string id)
        {
            if (!Has(id)) { return false; }

            if (_cache.TryGetValue(id, out var ret)) { return ret is T; }

            return false;
        }

        public T Get<T>(string id)
        {
            if (_cache.TryGetValue(id, out var ret)) { return (T) ret; }

            return default;
        }

        public T Set<T>(string id, T data)
        {
            _cache[id] = data;

            return data;
        }

        public bool Is(string id, Type type)
        {
            if (!Has(id)) { return false; }

            if (_cache.TryGetValue(id, out var ret)) { return ret.GetType() == type; }

            return false;
        }

        public object Get(string id)
        {
            return _cache.TryGetValue(id, out var ret) ? ret : null;
        }

        public object Set(string id, object obj)
        {
            _cache[id] = obj;
            return obj;
        }
    }
}