using UnityEngine;

namespace Hermit.Injection
{
    public abstract class MonoModule : MonoBehaviour, IModule
    {
        public abstract void RegisterBindings(IDependencyContainer Container);

        public virtual void Initialization(IDependencyContainer Container) { }
    }
}