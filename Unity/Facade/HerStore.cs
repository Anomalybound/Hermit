using System;
using System.Collections.Generic;
using Hermit.Service.Stores;

namespace Hermit
{
    public partial class Her
    {
        private readonly Dictionary<string, IStore> _stores = new Dictionary<string, IStore>();

        private IStore GlobalStore => _stores["Global"];

        #region Glaobal Store

        public string StoreId { get; }

        public static bool Has(string id)
        {
            return Current.GlobalStore.Has(id);
        }

        public static bool Is<T>(string id)
        {
            return Current.GlobalStore.Is<T>(id);
        }

        public static T Get<T>(string id)
        {
            return Current.GlobalStore.Get<T>(id);
        }

        public static T Set<T>(string id, T data)
        {
            return Current.GlobalStore.Set(id, data);
        }

        public static bool Is(string id, Type type)
        {
            return Current.GlobalStore.Is(id, type);
        }

        public static object Get(string id)
        {
            return Current.GlobalStore.Get(id);
        }

        public static object Set(string id, object obj)
        {
            return Current.GlobalStore.Set(id, obj);
        }

        #endregion

        #region Store Conainer

        public static IStore GetStore(string storeId)
        {
            if (Current._stores.TryGetValue(storeId, out var store)) { return store; }

            store = Create<IStore>();
            store.SetStoreId(storeId);

            Current._stores.Add(storeId, store);

            return store;
        }

        #endregion
    }
}