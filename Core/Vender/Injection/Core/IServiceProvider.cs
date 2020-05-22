namespace Hermit.Injection.Core
{
    public interface IServiceProvider
    {
        void RegisterBindings(IDependencyContainer container);

        void Initialization(IDependencyContainer container);
    }
}