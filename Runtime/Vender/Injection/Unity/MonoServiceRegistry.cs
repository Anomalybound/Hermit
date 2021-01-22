using UnityEngine;

namespace Hermit.Injection
{
    public abstract class MonoServiceRegistry : MonoBehaviour, IServiceRegistry
    {
        public abstract void RegisterBindings(IDependencyContainer container);

        public virtual void Initialization(IDependencyContainer container) { }
    }
}