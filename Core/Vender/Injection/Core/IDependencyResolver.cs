using System;

namespace Hermit.Injection
{
    public interface IDependencyResolver
    {
        object Create(Type type, string id = null);

        T Create<T>(string id = null) where T : class;

        object Instance(Type contract, string id = null);

        T Instance<T>(string id = null) where T : class;

        object Singleton(Type contract, string id = null);

        T Singleton<T>(string id = null) where T : class;

        object Resolve(Type contract, string id = null);

        T Resolve<T>(string id = null) where T : class;

        bool Has(Type contract, string id = null);

        bool Has<T>(string id = null) where T : class;

        T Inject<T>(T target);
    }
}
