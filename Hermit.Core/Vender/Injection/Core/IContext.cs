namespace Hermit.Injection
{
    public interface IContext : IDependencyResolver
    {
        IDependencyContainer Container { get; }
    }
}