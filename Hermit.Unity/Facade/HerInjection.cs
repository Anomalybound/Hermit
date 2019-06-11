using System;
using UnityEngine;

namespace Hermit
{
    public partial class Her
    {
        #region Static Facade Methods

        public static object Create(Type type, string id = null)
        {
            return Current.container.Create(type, id);
        }

        public static object Instance(Type type, string id = null)
        {
            return Current.container.Instance(type, id);
        }

        public static object Singleton(Type type, string id = null)
        {
            return Current.container.Singleton(type, id);
        }

        public static object Resolve(Type type, string id = null)
        {
            return Current.container.Resolve(type, id);
        }

        public static T Create<T>(string id = null) where T : class
        {
            return Current.container.Create<T>(id);
        }

        public static T Singleton<T>(string id = null) where T : class
        {
            return Current.container.Singleton<T>(id);
        }

        public static T Instance<T>(string id = null) where T : class
        {
            return Current.container.Instance<T>(id);
        }

        public static T Resolve<T>(string id = null) where T : class
        {
            return Current.container.Resolve<T>(id);
        }

        public static T Inject<T>(T target)
        {
            return Current.container.Inject(target);
        }

        public static void InjectGameObject(GameObject target)
        {
            var container = Current.container;
            var behaviours = target.GetComponents<MonoBehaviour>();
            foreach (var monoBehaviour in behaviours) { container.Inject(monoBehaviour); }
        }

        #endregion
    }
}