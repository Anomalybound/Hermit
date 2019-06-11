using System;

namespace Hermit.Injection
{
    public interface IDependencyContainer : IDisposable, IDependencyBinder, IDependencyResolver
    {
        #region Container

        IDependencyContainer MountModule(params IServiceProvider[] serviceProviders);

        void Build();

        #endregion
    }
}