namespace Hermit.Injection
{
    public interface IServiceProvider
    {
        void RegisterBindings(IDependencyContainer container);

        void Initialization(IDependencyContainer container);
    }
}