using System;
using UnityEngine;

namespace Hermit
{
    public partial class App
    {
        public static bool AutoRegister => I.GeneralSettings.AutoRegisterEvents;

        #region Static Facade Methods

        public static object Create(Type type, string id = null)
        {
            var instance = I.RootContainer.Create(type, id);
            if (instance != null && AutoRegister) { I.EventBroker.Register(instance); }

            return instance;
        }

        public static object Instance(Type type, string id = null)
        {
            var instance = I.RootContainer.Instance(type, id);
            if (instance != null && AutoRegister) { I.EventBroker.Register(instance); }

            return instance;
        }

        public static object Singleton(Type type, string id = null)
        {
            var instance = I.RootContainer.Singleton(type, id);
            if (instance != null && AutoRegister) { I.EventBroker.Register(instance); }

            return instance;
        }

        public static object Resolve(Type type, string id = null)
        {
            var instance = I.RootContainer.Resolve(type, id);
            if (instance != null && AutoRegister) { I.EventBroker.Register(instance); }

            return instance;
        }

        public static T Create<T>(string id = null) where T : class
        {
            var instance = I.RootContainer.Create<T>(id);
            if (instance != null && AutoRegister) { I.EventBroker.Register(instance); }

            return instance;
        }

        public static T Singleton<T>(string id = null) where T : class
        {
            var instance = I.RootContainer.Singleton<T>(id);
            if (instance != null && AutoRegister) { I.EventBroker.Register(instance); }

            return instance;
        }

        public static T Instance<T>(string id = null) where T : class
        {
            var instance = I.RootContainer.Instance<T>(id);
            if (instance != null && AutoRegister) { I.EventBroker.Register(instance); }

            return instance;
        }

        public static T Resolve<T>(string id = null) where T : class
        {
            var instance = I.RootContainer.Resolve<T>(id);

            if (instance != null && AutoRegister) { I.EventBroker.Register(instance); }

            return instance;
        }

        public static T Inject<T>(T target)
        {
            var instance = I.RootContainer.Inject(target);
            if (instance != null && AutoRegister) { I.EventBroker.Register(instance); }

            return instance;
        }

        public static void InjectGameObject(GameObject target)
        {
            var context = I.RootContainer;
            var behaviours = target.GetComponents<MonoBehaviour>();
            foreach (var monoBehaviour in behaviours)
            {
                var instance = context.Inject(monoBehaviour);
                if (instance != null && AutoRegister) { I.EventBroker.Register(instance); }
            }
        }

        #endregion
    }
}
