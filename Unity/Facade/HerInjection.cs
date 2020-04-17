using System;
using Hermit.Common;
using UnityEngine;

namespace Hermit
{
    public partial class Her
    {
        public static bool AutoRegister => Current.GeneralSettings.AutoRegisterEvents;

        #region Static Facade Methods

        public static object Create(Type type, string id = null)
        {
            var instance = Current.Context.Create(type, id);
            if (instance != null && AutoRegister) { Current.EventBroker.Register(instance); }

            return instance;
        }

        public static object Instance(Type type, string id = null)
        {
            var instance = Current.Context.Instance(type, id);
            if (instance != null && AutoRegister) { Current.EventBroker.Register(instance); }

            return instance;
        }

        public static object Singleton(Type type, string id = null)
        {
            var instance = Current.Context.Singleton(type, id);
            if (instance != null && AutoRegister) { Current.EventBroker.Register(instance); }

            return instance;
        }

        public static object Resolve(Type type, string id = null)
        {
            var instance = Current.Context.Resolve(type, id);
            if (instance != null && AutoRegister) { Current.EventBroker.Register(instance); }

            return instance;
        }

        public static T Create<T>(string id = null) where T : class
        {
            var instance = Current.Context.Create<T>(id);
            if (instance != null && AutoRegister) { Current.EventBroker.Register(instance); }

            return instance;
        }

        public static T Singleton<T>(string id = null) where T : class
        {
            var instance = Current.Context.Singleton<T>(id);
            if (instance != null && AutoRegister) { Current.EventBroker.Register(instance); }

            return instance;
        }

        public static T Instance<T>(string id = null) where T : class
        {
            var instance = Current.Context.Instance<T>(id);
            if (instance != null && AutoRegister) { Current.EventBroker.Register(instance); }

            return instance;
        }

        public static T Resolve<T>(string id = null) where T : class
        {
            var instance = Current.Context.Resolve<T>(id);

            if (instance != null && AutoRegister) { Current.EventBroker.Register(instance); }

            return instance;
        }

        public static T Inject<T>(T target)
        {
            var instance = Current.Context.Inject(target);
            if (instance != null && AutoRegister) { Current.EventBroker.Register(instance); }

            return instance;
        }

        public static void InjectGameObject(GameObject target)
        {
            var context = Current.Context;
            var behaviours = target.GetComponents<MonoBehaviour>();
            foreach (var monoBehaviour in behaviours)
            {
                var instance = context.Inject(monoBehaviour);
                if (instance != null && AutoRegister) { Current.EventBroker.Register(instance); }
            }
        }

        #endregion
    }
}
