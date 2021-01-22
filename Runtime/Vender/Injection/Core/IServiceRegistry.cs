namespace Hermit.Injection
{
    public interface IServiceRegistry
    {
        void RegisterBindings(IDependencyContainer container);

        void Initialization(IDependencyContainer container);
    }
}