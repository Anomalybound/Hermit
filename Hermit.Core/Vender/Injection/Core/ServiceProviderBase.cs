namespace Hermit.Injection
{
    public abstract class ServiceProviderBase : IServiceProvider
    {
        public abstract void RegisterBindings(IDependencyContainer container);

        public virtual void Initialization(IDependencyContainer container) { }
    }
}