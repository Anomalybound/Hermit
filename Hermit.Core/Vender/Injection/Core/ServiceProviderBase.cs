namespace Hermit.Injection
{
    public abstract class ServiceProviderBase : IServiceProvider
    {
        public abstract void RegisterBindings(IDependencyContainer Container);

        public virtual void Initialization(IDependencyContainer Container) { }
    }
}