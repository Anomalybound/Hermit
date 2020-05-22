namespace Hermit.Injection.Core
{
    public interface IContext : IDependencyResolver
    {
        IDependencyContainer Container { get; }
    }
}