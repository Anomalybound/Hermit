using System;
using UnityEngine;

namespace Hermit
{
    public partial class Her
    {
        public static bool AutoRegister => I.GeneralSettings.AutoRegisterEvents;

        #region Static Facade Methods

        public static object Create(Type type, string id = null)
        {
            var instance = I.Context.Container.Create(type, id);
            if (instance != null && AutoRegister) { I.EventBroker.Register(instance); }

            return instance;
        }

        public static object Instance(Type type, string id = null)
        {
            var instance = I.Context.Container.Instance(type, id);
            if (instance != null && AutoRegister) { I.EventBroker.Register(instance); }

            return instance;
        }

        public static object Singleton(Type type, string id = null)
        {
            var instance = I.Context.Container.Singleton(type, id);
            if (instance != null && AutoRegister) { I.EventBroker.Register(instance); }

            return instance;
        }

        public static object Resolve(Type type, string id = null)
        {
            var instance = I.Context.Container.Resolve(type, id);
            if (instance != null && AutoRegister) { I.EventBroker.Register(instance); }

            return instance;
        }

        public static T Create<T>(string id = null) where T : class
        {
            var instance = I.Context.Container.Create<T>(id);
            if (instance != null && AutoRegister) { I.EventBroker.Register(instance); }

            return instance;
        }

        public static T Singleton<T>(string id = null) where T : class
        {
            var instance = I.Context.Container.Singleton<T>(id);
            if (instance != null && AutoRegister) { I.EventBroker.Register(instance); }

            return instance;
        }

        public static T Instance<T>(string id = null) where T : class
        {
            var instance = I.Context.Container.Instance<T>(id);
            if (instance != null && AutoRegister) { I.EventBroker.Register(instance); }

            return instance;
        }

        public static T Resolve<T>(string id = null) where T : class
        {
            var instance = I.Context.Container.Resolve<T>(id);

            if (instance != null && AutoRegister) { I.EventBroker.Register(instance); }

            return instance;
        }

        public static T Inject<T>(T target)
        {
            var instance = I.Context.Container.Inject(target);
            if (instance != null && AutoRegister) { I.EventBroker.Register(instance); }

            return instance;
        }

        public static void InjectGameObject(GameObject target)
        {
            var context = I.Context.Container;
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
