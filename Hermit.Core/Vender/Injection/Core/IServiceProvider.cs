namespace Hermit.Injection
{
    public interface IServiceProvider
    {
        void RegisterBindings(IDependencyContainer Container);

        void Initialization(IDependencyContainer Container);
    }
}