using System;
using System.Collections.Generic;
using Hermit.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hermit.Injection
{
    [ScriptOrder(-10000)]
    public class SceneContext : MonoBehaviour, IContext
    {
        [SerializeField] protected bool injectSceneObjects;

        [SerializeField] protected MonoServiceProvider[] serviceProviders = { };

        protected IContext Context { get; private set; }

        public IDependencyContainer Container { get; private set; }

        protected readonly List<ServiceProviderBase> serviceProviderBases = new List<ServiceProviderBase>();

        protected virtual void Awake()
        {
            Context = new ContextBase();
            Container = Context.Container;
            
            RegisterServices();
            InitServices();
        }

        protected virtual void RegisterServices()
        {
            HermitEvent.Send(HermitEvent.ServiceRegisterStarted);

            RegisterInternalServiceProviders();
            RegisterMonoServiceProviders();

            Container.Build();

            HermitEvent.Send(HermitEvent.ServiceRegisterFinished);
        }

        protected virtual void InitServices()
        {
            HermitEvent.Send(HermitEvent.ServiceInjectionStarted);

            InitializeServiceProviders();

            if (injectSceneObjects)
            {
                for (var i = 0; i < SceneManager.sceneCount; i++)
                {
                    var scene = SceneManager.GetSceneAt(i);
                    var gos = scene.GetRootGameObjects();
                    foreach (var go in gos) { InjectGameObject(go); }
                }
            }

            HermitEvent.Send(HermitEvent.ServiceInjectionFinished);
        }

        protected virtual void RegisterServiceProviderBases() { }

        private void RegisterMonoServiceProviders()
        {
            if (serviceProviders == null || serviceProviders.Length == 0)
            {
                serviceProviders = GetComponentsInChildren<MonoServiceProvider>();
            }

            foreach (var provider in serviceProviders)
            {
                if (provider == null) { continue; }

                provider.RegisterBindings(Container);
            }
        }

        private void InitializeServiceProviders()
        {
            foreach (var provider in serviceProviders)
            {
                if (provider == null) { continue; }

                provider.Initialization(Container);
            }

            foreach (var serviceProviderBase in serviceProviderBases)
            {
                if (serviceProviderBase == null) { return; }

                serviceProviderBase.Initialization(Container);
            }
        }

        private void RegisterInternalServiceProviders()
        {
            // internal
            serviceProviderBases.Add(new HermitDataBindingServiceProvider());

            // custom service providers
            RegisterServiceProviderBases();

            // register
            foreach (var serviceProvider in serviceProviderBases) { serviceProvider.RegisterBindings(Container); }
        }

        #region Imeplementation of IContext

        public void InjectGameObject(GameObject targetGo)
        {
            var monoBehaviours = targetGo.GetComponents<MonoBehaviour>();
            foreach (var monoBehaviour in monoBehaviours)
            {
                if (monoBehaviour == null) { continue; }

                Context.Inject(monoBehaviour);
            }
        }

        public object Create(Type type, string id = null) => Context.Create(type, id);

        public T Create<T>(string id = null) where T : class => Context.Create<T>(id);

        public object Instance(Type type, string id = null) => Context.Instance(type, id);

        public T Instance<T>(string id = null) where T : class => Context.Instance<T>(id);

        public object Singleton(Type contract, string id = null) => Context.Singleton(contract, id);

        public T Singleton<T>(string id = null) where T : class => Context.Singleton<T>(id);

        public object Resolve(Type contract, string id = null) => Context.Resolve(contract, id);

        public T Resolve<T>(string id = null) where T : class => Context.Resolve<T>(id);

        public bool Has(Type contract, string id = null) => Context.Has(contract, id);

        public bool Has<T>(string id = null) where T : class => Context.Has<T>(id);

        public T Inject<T>(T target) => Context.Inject(target);

        #endregion
    }
}
