namespace Hermit.Injection
{
    public abstract class ModuleBase : IModule
    {
        public abstract void RegisterBindings(IDependencyContainer Container);

        public abstract void Initialization(IDependencyContainer Container);
    }
}