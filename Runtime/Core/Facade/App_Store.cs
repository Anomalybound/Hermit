using System;
using System.Collections.Generic;
using Hermit.Service.Stores;

namespace Hermit
{
    public partial class App
    {
        private readonly Dictionary<string, IStore> _stores = new Dictionary<string, IStore>();

        private IStore GlobalStore => _stores["Global"];

        #region Glaobal Store

        public string StoreId { get; private set; }

        public static bool Has(string id)
        {
            return I.GlobalStore.Has(id);
        }

        public static bool Is<T>(string id)
        {
            return I.GlobalStore.Is<T>(id);
        }

        public static T Get<T>(string id)
        {
            return I.GlobalStore.Get<T>(id);
        }

        public static T Set<T>(string id, T data)
        {
            return I.GlobalStore.Set(id, data);
        }

        public static bool Is(string id, Type type)
        {
            return I.GlobalStore.Is(id, type);
        }

        public static object Get(string id)
        {
            return I.GlobalStore.Get(id);
        }

        public static object Set(string id, object obj)
        {
            return I.GlobalStore.Set(id, obj);
        }

        #endregion

        #region Store Conainer

        public static IStore GetStore(string storeId)
        {
            if (I._stores.TryGetValue(storeId, out var store)) { return store; }

            store = Create<IStore>();
            store.SetStoreId(storeId);

            I._stores.Add(storeId, store);

            return store;
        }

        #endregion
    }
}
