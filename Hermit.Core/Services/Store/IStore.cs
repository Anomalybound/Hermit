using System;

namespace Hermit
{
    public interface IStore
    {
        string StoreId { get; }

        void SetStoreId(string id);

        bool Has(string id);

        bool Is<T>(string id);

        T Get<T>(string id);

        T Set<T>(string id, T data);

        bool Is(string id, Type type);

        object Get(string id);

        object Set(string id, object obj);
    }
}