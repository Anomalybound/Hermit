using System;
using UnityEngine;

namespace Hermit
{
    public partial class Her
    {
        #region Static Facade Methods

        public static object Create(Type type, string id = null)
        {
            return Current.Container.Create(type, id);
        }

        public static object Instance(Type type, string id = null)
        {
            return Current.Container.Instance(type, id);
        }

        public static object Singleton(Type type, string id = null)
        {
            return Current.Container.Singleton(type, id);
        }

        public static object Resolve(Type type, string id = null)
        {
            return Current.Container.Resolve(type, id);
        }

        public static T Create<T>(string id = null) where T : class
        {
            return Current.Container.Create<T>(id);
        }

        public static T Singleton<T>(string id = null) where T : class
        {
            return Current.Container.Singleton<T>(id);
        }

        public static T Instance<T>(string id = null) where T : class
        {
            return Current.Container.Instance<T>(id);
        }

        public static T Resolve<T>(string id = null) where T : class
        {
            return Current.Container.Resolve<T>(id);
        }

        public static T Inject<T>(T target)
        {
            return Current.Container.Inject(target);
        }

        public static void InjectGameObject(GameObject target)
        {
            var container = Current.Container;
            var behaviours = target.GetComponents<MonoBehaviour>();
            foreach (var monoBehaviour in behaviours) { container.Inject(monoBehaviour); }
        }

        #endregion
    }
}