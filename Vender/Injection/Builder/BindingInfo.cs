using System;
using System.Collections.Generic;

namespace Hermit.Injection
{
    public enum AsType
    {
        Singleton,
        Transient
    }

    public enum FromType
    {
        FromNew,
        FromInstance,
        FromMethods,
    }

    public class BindingInfo : IBindingInfo
    {
        public IDependencyContainer Container { get; }

        public List<Type> ContractTypes { get; }

        public Type ImplementType { get; protected set; }

        public object BindingInstance { get; private set; }

        public AsType As { get; private set; } = AsType.Singleton;

        public FromType From { get; protected set; } = FromType.FromNew;

        public Func<IDependencyContainer, object> BuildMethod { get; protected set; }

        public string BindingId { get; private set; }

        public bool BuildImmediately { get; private set; }

        public BindingInfo(IDependencyContainer container)
        {
            Container = container;
            ContractTypes = new List<Type>();
        }

        public IBindingInfo To(Type implementationType)
        {
            ToCheck(implementationType);
            ImplementType = implementationType;

            return this;
        }

        public IBindingInfo To<TImplementation>()
        {
            ToCheck(typeof(TImplementation));
            ImplementType = typeof(TImplementation);

            return this;
        }

        public IBindingInfo AsSingleton()
        {
            As = AsType.Singleton;

            return this;
        }

        public IBindingInfo AsTransient()
        {
            As = AsType.Transient;

            return this;
        }

        public IBindingInfo FromNew()
        {
            From = FromType.FromNew;

            return this;
        }

        public IBindingInfo FromInstance(object instance)
        {
            BindingInstance = instance;
            From = FromType.FromInstance;

            return this;
        }

        public IBindingInfo FromMethod(Func<IDependencyContainer, object> method)
        {
            BuildMethod = method;
            From = FromType.FromMethods;

            return this;
        }

        public IBindingInfo WithId(string id)
        {
            BindingId = id;

            return this;
        }

        public void NonLazy()
        {
            BuildImmediately = true;
        }

        #region Check Functions

        // TODO: warning prompt
        protected static void ToCheck(Type type) { }

        #endregion
    }

    public class BindingInfo<TContract> : BindingInfo, IBindingInfo<TContract>
    {
        public BindingInfo(IDependencyContainer container) : base(container) { }

        public new IBindingInfo<TContract> To(Type implementType)
        {
            ToCheck(implementType);
            ImplementType = implementType;
            return this;
        }

        public new IBindingInfo<TContract> To<TImplement>()
        {
            ToCheck(typeof(TImplement));
            ImplementType = typeof(TImplement);
            return this;
        }

        public new IBindingInfo<TContract> AsSingleton()
        {
            base.AsSingleton();
            return this;
        }

        public new IBindingInfo<TContract> AsTransient()
        {
            base.AsTransient();
            return this;
        }

        public new IBindingInfo<TContract> FromNew()
        {
            base.FromNew();
            return this;
        }

        public IBindingInfo<TContract> FromInstance(TContract instance)
        {
            base.FromInstance(instance);
            return this;
        }

        public IBindingInfo<TContract> FromMethod(Func<IDependencyContainer, TContract> method)
        {
            BuildMethod = container => method(container);
            From = FromType.FromMethods;

            return this;
        }

        public new IBindingInfo<TContract> WithId(string id)
        {
            base.WithId(id);

            return this;
        }
    }
}