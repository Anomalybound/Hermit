using UnityEngine;

namespace Hermit.Injection
{
    public abstract class MonoServiceProvider : MonoBehaviour, IServiceProvider
    {
        public abstract void RegisterBindings(IDependencyContainer Container);

        public virtual void Initialization(IDependencyContainer Container) { }
    }
}