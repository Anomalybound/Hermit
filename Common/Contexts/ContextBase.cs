using System;
using Hermit.Injection;
using UnityEngine;

namespace Hermit.Common
{
    public class ContextBase : IContext
    {
        public IDependencyContainer Container { get; }

        public ContextBase()
        {
            Container = new DiContainer();

            if (Context.GlobalContext == null) { Context.SetCurrentContext(this); }
        }

        public object Create(Type type, string id = null)
        {
            return Container.Create(type, id);
        }

        public T Create<T>(string id = null) where T : class
        {
            return Container.Create<T>(id);
        }

        public object Instance(Type contract, string id = null)
        {
            return Container.Instance(contract, id);
        }

        public T Instance<T>(string id = null) where T : class
        {
            return Container.Instance<T>(id);
        }

        public object Singleton(Type contract, string id = null)
        {
            return Container.Singleton(contract, id);
        }

        public T Singleton<T>(string id = null) where T : class
        {
            return Container.Singleton<T>(id);
        }

        public object Resolve(Type contract, string id = null)
        {
            return Container.Resolve(contract, id);
        }

        public T Resolve<T>(string id = null) where T : class
        {
            return Container.Resolve<T>(id);
        }

        public T Inject<T>(T target)
        {
            return Container.Inject(target);
        }

        public void InjectGameObject(GameObject target)
        {
            var monoBehaviours = target.GetComponents<MonoBehaviour>();
            foreach (var monoBehaviour in monoBehaviours)
            {
                if (monoBehaviour == null) { continue; }

                Inject(monoBehaviour);
            }
        }
    }
}