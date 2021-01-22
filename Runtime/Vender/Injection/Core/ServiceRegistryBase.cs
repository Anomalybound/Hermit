namespace Hermit.Injection
{
    public abstract class ServiceRegistryBase : IServiceRegistry
    {
        public abstract void RegisterBindings(IDependencyContainer container);

        public virtual void Initialization(IDependencyContainer container) { }
    }
}