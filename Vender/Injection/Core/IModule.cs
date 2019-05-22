namespace Hermit.Injection
{
    public interface IModule
    {
        void RegisterBindings(IDependencyContainer Container);

        void Initialization(IDependencyContainer Container);
    }
}