//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//
//namespace Hermit.Injection
//{
//    public class DependencyContainer : IDependencyContainer
//    {
//        #region Constants
//
//        public static readonly bool ALLOW_OVERRIDE_BINDING = true;
//
//        #endregion
//
//        public readonly Dictionary<Type, Type> Types = new Dictionary<Type, Type>();
//
//        public readonly Dictionary<Type, List<MemberInfo>> _memberCaches = new Dictionary<Type, List<MemberInfo>>();
//
//        private readonly Dictionary<Type, object> _instanceCaches = new Dictionary<Type, object>();
//
//        private readonly List<IModule> _modules = new List<IModule>();
//
//        #region Helper
//
//        public DependencyContainer()
//        {
//            Bind<IDependencyContainer, DependencyContainer>(this);
//        }
//        
//        #region Binds
//        
//        #endregion
//
//        #region Generic Binding
//
//        public IBinderInfo BindInterfaces<TImplementation>()
//        {
//            throw new NotImplementedException();
//        }
//
//        public IBinderInfo<TImplementation> BindAll<TImplementation>()
//        {
//            var implementation = typeof(TImplementation);
//            var interfaces = implementation.GetInterfaces();
//            var binder = new BinderInfo<TImplementation>(this, implementation);
//            CheckBindingCache(implementation);
//            Types[implementation] = implementation;
//            foreach (var interfaceType in interfaces) { Types[interfaceType] = implementation; }
//
//            return binder;
//        }
//
//        public IBinderInfo<TContract> BindInstance<TContract>(TContract instance)
//        {
//            throw new NotImplementedException();
//        }
//
//        public IBinderInfo<TImplementation> BindAll<TImplementation>(TImplementation instance)
//        {
//            var implementation = typeof(TImplementation);
//            var interfaces = implementation.GetInterfaces();
//            var binder = new BinderInfo<TImplementation>(this, implementation);
//            CheckBindingCache(implementation);
//            Types[implementation] = implementation;
//            foreach (var interfaceType in interfaces) { Types[interfaceType] = implementation; }
//
//            if (instance != null) { AddSingleton(implementation, instance); }
//
//            return binder;
//        }
//
//        public IBinderInfo BindInstance(Type instanceType, object instance)
//        {
//            throw new NotImplementedException();
//        }
//
//        public IBinderInfo<TImplementation> Bind<TImplementation>()
//        {
//            return Bind<TImplementation, TImplementation>();
//        }
//
//        public IBinderInfo<TImplementation> Bind<TImplementation>(TImplementation instance)
//        {
//            return Bind<TImplementation, TImplementation>(instance);
//        }
//
//        public IBinderInfo<TImplementation> Bind<TContract, TImplementation>() where TImplementation : TContract
//        {
//            var binder = new BinderInfo<TImplementation>(this, typeof(TContract));
//            CheckBindingCache(typeof(TContract));
//            Types[typeof(TContract)] = typeof(TImplementation);
//            return binder;
//        }
//
//        public IBinderInfo<TImplementation> Bind<TContract, TImplementation>(TContract instance)
//            where TImplementation : TContract
//        {
//            var binder = new BinderInfo<TImplementation>(this, typeof(TContract));
//            CheckBindingCache(typeof(TContract));
//            Types[typeof(TContract)] = typeof(TImplementation);
//            if (instance != null) { AddSingleton(typeof(TContract), instance); }
//
//            return binder;
//        }
//
//        #endregion
//
//        #region Non Generic
//
//        public IBinderInfo BindInterfaces(Type implementationType)
//        {
//            throw new NotImplementedException();
//        }
//
//        public IBinderInfo BindAll(Type implementation)
//        {
//            return BindAll(implementation, null);
//        }
//
//        public IBinderInfo BindAll(Type implementation, object instance)
//        {
//            var interfaces = implementation.GetInterfaces();
//            var binder = new BinderInfo(this, implementation);
//            CheckBindingCache(implementation);
//            Types[implementation] = implementation;
//            if (instance != null) { AddSingleton(implementation, instance); }
//
//            foreach (var interfaceType in interfaces) { Types[interfaceType] = implementation; }
//
//            return binder;
//        }
//
//        public IBinderInfo Bind(Type implementation)
//        {
//            return Bind(implementation, implementation);
//        }
//
//        public IBinderInfo Bind(Type implementation, object instance)
//        {
//            return Bind(implementation, implementation, instance);
//        }
//
//        public IBinderInfo Bind(Type contract, Type implementation)
//        {
//            return Bind(contract, implementation, null);
//        }
//
//        public IBinderInfo Bind(Type contract, Type implementation, object instance)
//        {
//            var binder = new BinderInfo(this, contract);
//            CheckBindingCache(contract);
//            Types[contract] = implementation;
//            if (instance != null) { AddSingleton(contract, instance); }
//
//            return binder;
//        }
//
//        #endregion
//
//        public T Resolve<T>() where T : class
//        {
//            return Resolve(typeof(T)) as T;
//        }
//
//        public object Resolve(Type contract)
//        {
//            var instance = InternalResolve(contract);
//            Inject(instance);
//            return instance;
//        }
//
//        public object Instance(Type type)
//        {
//            throw new NotImplementedException();
//        }
//
//        public T Instance<T>() where T : class
//        {
//            throw new NotImplementedException();
//        }
//
//        public T Singleton<T>() where T : class
//        {
//            return Singleton(typeof(T)) as T;
//        }
//
//        public object Singleton(Type contract)
//        {
//            var instance = InternalResolve(contract);
//            Inject(instance);
//            return instance;
//        }
//
//        public T Inject<T>(T target)
//        {
//            var objType = target.GetType();
//
//            if (!_memberCaches.TryGetValue(objType, out var injectedMembers))
//            {
//                injectedMembers = new List<MemberInfo>();
//                var members = objType.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
//                    .Where(x => x.CustomAttributes.Any(a => a.AttributeType == typeof(Inject)));
//
//                var memberInfos = members as MemberInfo[] ?? members.ToArray();
//                injectedMembers.AddRange(memberInfos);
//
//                _memberCaches.Add(objType, injectedMembers);
//            }
//
//            var memberInfoCaches = _memberCaches[objType];
//
//            foreach (var memberInfo in memberInfoCaches)
//            {
//                object instance = null;
//                switch (memberInfo.MemberType)
//                {
//                    case MemberTypes.Field:
//                        var fieldInfo = memberInfo as FieldInfo;
//                        instance = InternalResolve(fieldInfo.FieldType);
//                        fieldInfo.SetValue(target, instance);
//                        break;
//                    case MemberTypes.Method:
//                        var methodInfo = memberInfo as MethodInfo;
//                        var parameters = methodInfo.GetParameters();
//                        var invokeParameter = new object[parameters.Length];
//                        for (var i = 0; i < parameters.Length; i++)
//                        {
//                            var parameter = parameters[i];
//                            invokeParameter[i] = InternalResolve(parameter.ParameterType);
//                        }
//
//                        methodInfo.Invoke(target, invokeParameter);
//                        break;
//                    case MemberTypes.Property:
//                        var propertyInfo = memberInfo as PropertyInfo;
//                        if (propertyInfo.SetMethod != null)
//                        {
//                            instance = InternalResolve(propertyInfo.PropertyType);
//                            propertyInfo.SetValue(target, instance);
//                        }
//
//                        break;
//                }
//            }
//
//            return target;
//        }
//
//        public IDependencyContainer MountModule(params IModule[] modules)
//        {
//            _modules.AddRange(modules);
//
//            foreach (var module in modules) { module.RegisterBindings(); }
//
//            return this;
//        }
//
//        #endregion
//
//        #region Helpers
//
//        public bool ContainsSingleton(Type type)
//        {
//            return _instanceCaches.ContainsKey(type);
//        }
//
//        #endregion
//
//        #region Internal
//
//        private void CheckBindingCache(Type bindingType)
//        {
//            if (Types.ContainsKey(bindingType) && !ALLOW_OVERRIDE_BINDING)
//            {
//                throw new ApplicationException($"{bindingType} already bound.");
//            }
//        }
//
//        private T InternalResolve<T>(bool createMode = false)
//        {
//            return (T) InternalResolve(typeof(T), createMode);
//        }
//
//        private object InternalCreateInstance(Type contract) { }
//
//        private object InternalResolve(Type contract)
//        {
//            if (!Types.TryGetValue(contract, out var implementation))
//            {
//                throw new ApplicationException("Can't resolve a unregistered type: " + contract);
//            }
//
//            if (_transientTypes.Contains(contract) || !_instanceCaches.TryGetValue(contract, out var instance))
//            {
//                var constructor = implementation.GetConstructors()[0];
//                var parameterInfos = constructor.GetParameters();
//
//                if (parameterInfos.Length == 0)
//                {
//                    instance = Activator.CreateInstance(implementation);
//                    _instanceCaches.Add(contract, instance);
//                    Inject(instance);
//                    return instance;
//                }
//
//                var parameters = new List<object>(parameterInfos.Length);
//                foreach (var parameterInfo in parameterInfos)
//                {
//                    parameters.Add(InternalResolve(parameterInfo.ParameterType));
//                }
//
//                instance = constructor.Invoke(parameters.ToArray());
//
//                _instanceCaches.Add(contract, instance);
//            }
//
//            return instance;
//        }
//
//        #endregion
//
//        protected virtual void Dispose(bool disposing)
//        {
//            if (!disposing) { return; }
//
//            Types.Clear();
//            _memberCaches.Clear();
//            _instanceCaches.Clear();
//        }
//
//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }
//    }
//}