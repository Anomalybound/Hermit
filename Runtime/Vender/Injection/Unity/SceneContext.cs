using System.Collections.Generic;
using Hermit.ServiceProvider;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hermit.Injection
{
    [ScriptOrder(-10000)]
    public class SceneContext : MonoBehaviour, IContext
    {
        [SerializeField] protected bool injectSceneObjects;

        [SerializeField] protected MonoServiceRegistry[] serviceProviders = { };

        public IDependencyContainer Container { get; private set; }

        protected readonly List<ServiceRegistryBase> ServiceProviderBases = new List<ServiceRegistryBase>();

        protected virtual void Awake()
        {
            Container = new DiContainer();
            Contexts.SetActiveContext(this);
            
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
                serviceProviders = GetComponentsInChildren<MonoServiceRegistry>();
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

            foreach (var serviceProviderBase in ServiceProviderBases)
            {
                if (serviceProviderBase == null) { return; }

                serviceProviderBase.Initialization(Container);
            }
        }

        private void RegisterInternalServiceProviders()
        {
            // internal
            ServiceProviderBases.Add(new HermitDataBindingServiceRegistry());

            // custom service providers
            RegisterServiceProviderBases();

            // register
            foreach (var serviceProvider in ServiceProviderBases) { serviceProvider.RegisterBindings(Container); }
        }

        #region Imeplementation of IContext

        public void InjectGameObject(GameObject targetGo)
        {
            var monoBehaviours = targetGo.GetComponents<MonoBehaviour>();
            foreach (var monoBehaviour in monoBehaviours)
            {
                if (monoBehaviour == null) { continue; }

                Container.Inject(monoBehaviour);
            }
        }
        public T Inject<T>(T target) => Container.Inject(target);

        #endregion
    }
}
