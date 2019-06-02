using System;

namespace Hermit.Injection
{
    public interface IBindingInfo
    {
        #region To

        IBindingInfo To(Type implementationType);

        IBindingInfo To<TImplementation>();

        #endregion

        #region AsScope

        IBindingInfo AsSingleton();

        IBindingInfo AsTransient();

        #endregion

        #region From

        IBindingInfo FromNew();

        IBindingInfo FromInstance(object instance);

        IBindingInfo FromMethod(Func<IDependencyContainer, object> method);

        #endregion

        #region Identifier

        IBindingInfo WithId(string id);

        #endregion

        #region Non Lazy

        void NonLazy();

        #endregion
    }

    public interface IBindingInfo<in TContract> : IBindingInfo
    {
        #region To

        new IBindingInfo<TContract> To(Type implementType);

        new IBindingInfo<TContract> To<TImplement>();

        #endregion

        #region AsScope

        new IBindingInfo<TContract> AsSingleton();

        new IBindingInfo<TContract> AsTransient();

        #endregion

        #region From

        new IBindingInfo<TContract> FromNew();

        IBindingInfo<TContract> FromInstance(TContract instance);

        IBindingInfo<TContract> FromMethod(Func<IDependencyContainer, TContract> method);

        #endregion

        #region Identifier

        new IBindingInfo<TContract> WithId(string id);

        #endregion
    }
}