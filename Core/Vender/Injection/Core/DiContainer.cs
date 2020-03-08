using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hermit.Injection
{
    public class DiContainer : IDependencyContainer
    {
        protected readonly List<IBindingInfo> BinderInfos = new List<IBindingInfo>();

        protected readonly List<IServiceProvider> Modules = new List<IServiceProvider>();

        protected readonly Dictionary<(string, Type), object> SingletonInstance =
            new Dictionary<(string, Type), object>();

        protected readonly Dictionary<(string, Type), List<BindingInfo>> ContractTypeLookup =
            new Dictionary<(string, Type), List<BindingInfo>>();

        protected readonly Dictionary<Type, List<(MemberInfo memberInfo, string id)>> MemberInfoCaches =
            new Dictionary<Type, List<(MemberInfo, string)>>();

        protected readonly Queue<object> PendingInjectionQueue = new Queue<object>();

        protected const string DefaultInjectionKey = "Default";

        public IDependencyContainer MountModule(params IServiceProvider[] serviceProviders)
        {
            Modules.AddRange(serviceProviders);

            foreach (var module in serviceProviders) { module.RegisterBindings(this); }

            return this;
        }

        /// <summary>
        /// Build dependency map, should be called by the framework itself.
        /// </summary>
        public void Build()
        {
            foreach (var binderInfo in BinderInfos) { Build(binderInfo); }
        }

        public void Build(IBindingInfo bindingInfo)
        {
            if (!(bindingInfo is BindingInfo info)) { return; }

            if (info.ContractTypes.Count <= 0)
            {
                throw new Exception($"Fatal error, no contract types included in building binding info.");
            }

            // add contract type as default if implement type is not assigned
            if (info.ImplementType == null) { info.To(info.ContractTypes[0]); }

            // add default injection key if null
            if (string.IsNullOrEmpty(info.BindingId)) { info.WithId(DefaultInjectionKey); }

            foreach (var contractType in info.ContractTypes)
            {
                if (ContractTypeLookup.ContainsKey((info.BindingId, contractType)))
                {
                    BindEnumerableType(info.BindingId, contractType, info);

                    // set active resolve result to newest one
                    var resultList = ContractTypeLookup[(info.BindingId, contractType)];
                    resultList.Clear();
                    resultList.Add(info);
                } else { ContractTypeLookup.Add((info.BindingId, contractType), new List<BindingInfo> {info}); }
            }

            if (info.BuildImmediately) { Resolve(info); }
        }

        private void BindEnumerableType(string id, Type contractType, BindingInfo info)
        {
            var enumerableTypes = GetEnumerableTypes(contractType);
            var previousBindingInfos = ContractTypeLookup[(id, contractType)];
            var newBindingInfos = new List<BindingInfo> {info};
            newBindingInfos.AddRange(previousBindingInfos);

            foreach (var enumerableType in enumerableTypes)
            {
                if (!ContractTypeLookup.TryGetValue((id, enumerableType), out var binderInfos))
                {
                    ContractTypeLookup.Add((id, enumerableType), newBindingInfos);
                    continue;
                }

                if (!binderInfos.Contains(info)) { binderInfos.Add(info); }
            }
        }

        protected readonly Dictionary<Type, Type[]> EnumerableTypeCache = new Dictionary<Type, Type[]>();

        private IEnumerable<Type> GetEnumerableTypes(Type contractType)
        {
            if (EnumerableTypeCache.TryGetValue(contractType, out var ret)) { return ret; }

            var listType = typeof(List<>).MakeGenericType(contractType);
            var iListType = typeof(IList<>).MakeGenericType(contractType);
            var iCollectionType = typeof(ICollection<>).MakeGenericType(contractType);
            var iEnumerableType = typeof(IEnumerable<>).MakeGenericType(contractType);
            var iReadOnlyListType = typeof(IReadOnlyList<>).MakeGenericType(contractType);
            var iReadOnlyCollectionType = typeof(IReadOnlyCollection<>).MakeGenericType(contractType);

            var types = new[]
            {
                listType, iListType, iCollectionType,
                iEnumerableType, iReadOnlyListType, iReadOnlyCollectionType
            };

            EnumerableTypeCache.Add(contractType, types);

            return types;
        }

        #region Binds

        public IBindingInfo Bind(Type contractType)
        {
            var info = new BindingInfo(this);
            BindContractType(info, contractType);
            BinderInfos.Add(info);

            return info;
        }

        public IBindingInfo BindInterfaces(Type implementationType)
        {
            var info = new BindingInfo(this);
            var interfaces = implementationType.GetInterfaces();
            foreach (var interfaceType in interfaces) { BindContractType(info, interfaceType); }

            BinderInfos.Add(info);

            return info;
        }

        public IBindingInfo BindAll(Type implementType)
        {
            var info = new BindingInfo(this);
            var interfaces = implementType.GetInterfaces();

            BindContractType(info, implementType);
            foreach (var interfaceType in interfaces) { BindContractType(info, interfaceType); }

            BinderInfos.Add(info);

            return info.To(implementType);
        }

        public IBindingInfo BindInstance(Type instanceType, object instance)
        {
            return Bind(instanceType).FromInstance(instance);
        }

        public IBindingInfo<TContract> Bind<TContract>()
        {
            var info = new BindingInfo<TContract>(this);
            BindContractType(info, typeof(TContract));
            BinderInfos.Add(info);

            return info;
        }

        public IBindingInfo<TImplementation> BindInterfaces<TImplementation>()
        {
            var info = new BindingInfo<TImplementation>(this);
            var interfaces = typeof(TImplementation).GetInterfaces();

            foreach (var interfaceType in interfaces) { BindContractType(info, interfaceType); }

            BinderInfos.Add(info);

            return info;
        }

        public IBindingInfo<TImplementation> BindAll<TImplementation>()
        {
            var info = new BindingInfo<TImplementation>(this);
            var interfaces = typeof(TImplementation).GetInterfaces();

            BindContractType(info, typeof(TImplementation));
            foreach (var interfaceType in interfaces) { BindContractType(info, interfaceType); }

            BinderInfos.Add(info);

            return info.To<TImplementation>();
        }

        public IBindingInfo<TImplementation> BindInstance<TImplementation>(TImplementation instance)
        {
            return Bind<TImplementation>().FromInstance(instance);
        }

        #endregion

        public object Instance(Type contract, string id = null) => OperationHelper(Instance, contract, id);

        public object Resolve(Type contract, string id = null) => OperationHelper(Resolve, contract, id);

        public object Singleton(Type contract, string id = null) => OperationHelper(Singleton, contract, id);

        public object Create(Type type, string id = null)
        {
            if (string.IsNullOrEmpty(id)) { id = DefaultInjectionKey; }

            return ContractTypeLookup.ContainsKey((id, type))
                ? Instance(type, id)
                : CreateInstanceFromNew(type);
        }

        public bool Has(Type contract, string id = null)
        {
            if (string.IsNullOrEmpty(id)) { id = DefaultInjectionKey; }

            return ContractTypeLookup.ContainsKey((id, contract));
        }

        public T Inject<T>(T target)
        {
            var contractType = target.GetType();

            if (!MemberInfoCaches.TryGetValue(contractType, out var injectedMembers))
            {
                injectedMembers = new List<(MemberInfo, string)>();
                var members = contractType
                    .GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(x => x.CustomAttributes.Any(a => a.AttributeType == typeof(Inject)));

                var memberInfos = members as MemberInfo[] ?? members.ToArray();
                var mappedInfos =
                    memberInfos.Select(m => ValueTuple.Create(m, m.GetCustomAttribute<Inject>().BindingId));
                injectedMembers.AddRange(mappedInfos);

                MemberInfoCaches.Add(contractType, injectedMembers);
            }

            var memberInfoCaches = MemberInfoCaches[contractType];

            foreach (var (memberInfo, id) in memberInfoCaches)
            {
                object instance;
                switch (memberInfo.MemberType)
                {
                    case MemberTypes.Field:
                        var fieldInfo = memberInfo as FieldInfo;
                        if (fieldInfo != null)
                        {
                            instance = Resolve(fieldInfo.FieldType, id);
                            fieldInfo.SetValue(target, instance);
                        }

                        break;
                    case MemberTypes.Property:
                        var propertyInfo = memberInfo as PropertyInfo;
                        if (propertyInfo != null && propertyInfo.SetMethod != null)
                        {
                            instance = Resolve(propertyInfo.PropertyType, id);
                            propertyInfo.SetValue(target, instance);
                        }

                        break;
                    case MemberTypes.Method:
                        var methodInfo = memberInfo as MethodInfo;
                        if (methodInfo != null)
                        {
                            var parameters = methodInfo.GetParameters();

                            var invokeParameter = new object[parameters.Length];
                            for (var i = 0; i < parameters.Length; i++)
                            {
                                var parameter = parameters[i];
                                invokeParameter[i] = Resolve(parameter.ParameterType);
                            }

                            methodInfo.Invoke(target, invokeParameter);
                        }

                        break;
                }
            }

            return target;
        }

        public T Create<T>(string id = null) where T : class => Create(typeof(T), id) as T;

        public T Instance<T>(string id = null) where T : class => Instance(typeof(T), id) as T;

        public T Singleton<T>(string id = null) where T : class => Singleton(typeof(T), id) as T;

        public T Resolve<T>(string id = null) where T : class => Resolve(typeof(T), id) as T;

        public bool Has<T>(string id = null) where T : class => Has(typeof(T), id);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) { return; }

            MemberInfoCaches.Clear();
            BinderInfos.Clear();
            Modules.Clear();
            SingletonInstance.Clear();
            ContractTypeLookup.Clear();
            PendingInjectionQueue.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region Internal Implementations

        protected object OperationHelper(Func<BindingInfo, object> operation, Type contractType, string id)
        {
            // add default injection key if null
            if (string.IsNullOrEmpty(id)) { id = DefaultInjectionKey; }

            if (!ContractTypeLookup.TryGetValue((id, contractType), out var binderInfos))
            {
                if (string.IsNullOrEmpty(id)) { throw new Exception($"Resolve type: {contractType} failed!"); }

                throw new Exception($"Resolve type: {contractType} with id: {id} failed!");
            }

            if (binderInfos.Count == 0) { throw new Exception($"Can not resolve type: {contractType}."); }

            if (binderInfos.Count == 1) { return operation.Invoke(binderInfos[0]); }

            // Resolving Enumerable Type
            var enumeratorType = typeof(List<>).MakeGenericType(contractType.GenericTypeArguments[0]);

            if (!(Activator.CreateInstance(enumeratorType) is IList list))
            {
                throw new Exception($"Something went wrong when resolving of type: {contractType}.");
            }

            foreach (var binderInfo in binderInfos)
            {
                var instance = operation.Invoke(binderInfo);
                list.Add(instance);
            }

            return list;
        }

        protected object Resolve(BindingInfo info)
        {
            switch (info.As)
            {
                case AsType.Singleton:
                    return Singleton(info);

                case AsType.Transient:
                    return Instance(info);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected object Instance(BindingInfo info)
        {
            switch (info.From)
            {
                case FromType.FromNew:
                    return CreateInstanceFromNew(info.ImplementType);
                case FromType.FromInstance:
                    return info.BindingInstance;
                case FromType.FromMethods:
                    return info.BuildMethod.Invoke(this);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected object Singleton(BindingInfo info)
        {
            var id = info.BindingId;
            var implementType = info.ImplementType;

            // No instances were found, create new instance
            if (SingletonInstance.TryGetValue((id, implementType), out var ret)) { return ret; }

            ret = Instance(info);
            SingletonInstance.Add((id, implementType), ret);

            return ret;
        }

        #endregion

        #region Helpers

        protected object CreateInstanceFromNew(Type targetType)
        {
            var constructors =
                targetType.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Default);

            // Only react to the first available constructor
            for (var j = 0; j < constructors.Length;)
            {
                var constructor = constructors[j];
                var parameterInfos = constructor.GetParameters();

                if (parameterInfos.Length == 0) { return Activator.CreateInstance(targetType); }

                var parameters = new List<object>(parameterInfos.Length);
                var resolvedData = parameterInfos.Select(parameterInfo => Resolve(parameterInfo.ParameterType));
                parameters.AddRange(resolvedData);

                return Inject(constructor.Invoke(parameters.ToArray()));
            }

            throw new Exception($"Unable to use From New Scope on {targetType}, no suitable Constructor");
        }

        public void BindContractType(BindingInfo info, Type contractType)
        {
            if (info.ContractTypes.Contains(contractType)) { return; }

            info.ContractTypes.Add(contractType);
        }

        #endregion
    }
}
