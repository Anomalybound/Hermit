using System;
using System.Collections.Generic;

namespace Hermit.Service.Stores
{
    public interface IStore
    {
        string StoreId { get; }

        List<string> Keys { get; }

        void SetStoreId(string id);

        bool Has(string key);

        bool Is<T>(string key);

        T Get<T>(string key);

        T Set<T>(string key, T data);

        bool Is(string key, Type type);

        object Get(string key);

        object Set(string key, object obj);

        void Remove(string key);

        void ClearAll();
    }
}