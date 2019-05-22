using System;
using UnityEngine;

namespace Hermit
{
    public partial class Her
    {
        #region Static Facade Methods

        public static object Create(Type type, string id = null)
        {
            return Current._container.Create(type, id);
        }

        public static object Instance(Type type, string id = null)
        {
            return Current._container.Instance(type, id);
        }

        public static object Singleton(Type type, string id = null)
        {
            return Current._container.Singleton(type, id);
        }

        public static object Resolve(Type type, string id = null)
        {
            return Current._container.Resolve(type, id);
        }
        
        public static object Create<T>(string id = null) where T : class
        {
            return Current._container.Create<T>(id);
        }

        public static T Singleton<T>(string id = null) where T : class
        {
            return Current._container.Singleton<T>(id);
        }

        public static T Instance<T>(string id = null) where T : class
        {
            return Current._container.Instance<T>(id);
        }

        public static T Resolve<T>(string id = null) where T : class
        {
            return Current._container.Resolve<T>(id);
        }

        public static T Inject<T>(T target)
        {
            return Current._container.Inject(target);
        }

        public static void InjectGameObject(GameObject target)
        {
            Current._container.InjectGameObject(target);
        }

        #endregion
    }
}