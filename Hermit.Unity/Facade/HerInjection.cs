using System;
using Hermit.Common;
using UnityEngine;

namespace Hermit
{
    public partial class Her
    {
        public static bool AutoRegister => Current.GeneralSettings.RegisterEventWhileInjection;

        #region Static Facade Methods

        public static object Create(Type type, string id = null)
        {
            var instance = Current.Container.Create(type, id);
            if (AutoRegister) { Current.EventBroker.Register(instance); }

            return instance;
        }

        public static object Instance(Type type, string id = null)
        {
            var instance = Current.Container.Instance(type, id);
            if (AutoRegister) { Current.EventBroker.Register(instance); }

            return instance;
        }

        public static object Singleton(Type type, string id = null)
        {
            var instance = Current.Container.Singleton(type, id);
            if (AutoRegister) { Current.EventBroker.Register(instance); }

            return instance;
        }

        public static object Resolve(Type type, string id = null)
        {
            var instance = Current.Container.Resolve(type, id);
            if (AutoRegister) { Current.EventBroker.Register(instance); }

            return instance;
        }

        public static T Create<T>(string id = null) where T : class
        {
            var instance = Current.Container.Create<T>(id);
            if (AutoRegister) { Current.EventBroker.Register(instance); }

            return instance;
        }

        public static T Singleton<T>(string id = null) where T : class
        {
            var instance = Current.Container.Singleton<T>(id);
            if (AutoRegister) { Current.EventBroker.Register(instance); }

            return instance;
        }

        public static T Instance<T>(string id = null) where T : class
        {
            var instance = Current.Container.Instance<T>(id);
            if (AutoRegister) { Current.EventBroker.Register(instance); }

            return instance;
        }

        public static T Resolve<T>(string id = null) where T : class
        {
            var instance = Current.Container.Resolve<T>(id);
            if (AutoRegister) { Current.EventBroker.Register(instance); }

            return instance;
        }

        public static T Inject<T>(T target)
        {
            var instance = Current.Container.Inject(target);
            if (AutoRegister) { Current.EventBroker.Register(instance); }

            return instance;
        }

        public static void InjectGameObject(GameObject target)
        {
            var container = Current.Container;
            var behaviours = target.GetComponents<MonoBehaviour>();
            foreach (var monoBehaviour in behaviours)
            {
                var instance = container.Inject(monoBehaviour);
                if (AutoRegister) { Current.EventBroker.Register(instance); }
            }
        }

        #endregion
    }
}