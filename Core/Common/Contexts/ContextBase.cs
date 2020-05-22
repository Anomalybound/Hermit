using System;
using Hermit.Injection;
using Hermit.Injection.Core;

namespace Hermit.Common
{
    public class ContextBase : IContext
    {
        public IDependencyContainer Container { get; }

        public ContextBase()
        {
            Container = new DiContainer();

            if (Contexts.GlobalContext == null) { Contexts.SetCurrentContext(this); }
        }

        public object Create(Type type, string id = null) => Container.Create(type, id);

        public T Create<T>(string id = null) where T : class => Container.Create<T>(id);

        public object Instance(Type contract, string id = null) => Container.Instance(contract, id);

        public T Instance<T>(string id = null) where T : class => Container.Instance<T>(id);

        public object Singleton(Type contract, string id = null) => Container.Singleton(contract, id);

        public T Singleton<T>(string id = null) where T : class => Container.Singleton<T>(id);

        public object Resolve(Type contract, string id = null) => Container.Resolve(contract, id);

        public T Resolve<T>(string id = null) where T : class => Container.Resolve<T>(id);

        public bool Has(Type contract, string id = null) => Container.Has(contract, id);

        public bool Has<T>(string id = null) where T : class => Container.Has<T>(id);

        public T Inject<T>(T target) => Container.Inject(target);
    }
}
