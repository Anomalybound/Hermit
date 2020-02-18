using System;

namespace Hermit.Injection
{
    public interface IDependencyBinder
    {
        #region Bind

        IBindingInfo Bind(Type contractType);

        IBindingInfo BindInterfaces(Type implementationType);

        IBindingInfo BindAll(Type implementType);

        IBindingInfo BindInstance(Type instanceType, object instance);

        IBindingInfo<TContract> Bind<TContract>();

        IBindingInfo<TImplementation> BindInterfaces<TImplementation>();

        IBindingInfo<TImplementation> BindAll<TImplementation>();

        IBindingInfo<TImplementation> BindInstance<TImplementation>(TImplementation instance);

        #endregion
    }
}