using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace Hermit.Injection
{
    [ScriptOrder(-10000)]
    public class SceneContext : MonoBehaviour, IContext
    {
        public virtual IDependencyContainer Container { get; } = new DiContainer();

        [SerializeField]
        protected bool injectSceneObjects;

        [SerializeField]
        protected MonoServiceProvider[] serviceProviders = { };

        protected virtual void Awake()
        {
            RegisterServices();
            InitServices();
        }

        protected virtual void RegisterServices()
        {
            if (Context.GlobalContext == null) { Context.SetCurrentContext(this); }

            if (serviceProviders == null || serviceProviders.Length == 0)
            {
                serviceProviders = GetComponentsInChildren<MonoServiceProvider>();
            }

            var sw = Stopwatch.StartNew();
            sw.Start();

            foreach (var provider in serviceProviders)
            {
                if (provider == null) { continue; }

                provider.RegisterBindings(Container);
            }

            Container.Build();
            sw.Stop();

            Debug.LogFormat($"Services registration finished, cost : {sw.ElapsedMilliseconds}ms. ");
        }

        protected virtual void InitServices()
        {
            var sw = Stopwatch.StartNew();
            sw.Start();
            foreach (var provider in serviceProviders)
            {
                if (provider == null) { continue; }

                provider.Initialization(Container);
            }

            if (injectSceneObjects)
            {
                for (var i = 0; i < SceneManager.sceneCount; i++)
                {
                    var scene = SceneManager.GetSceneAt(i);
                    var gos = scene.GetRootGameObjects();
                    foreach (var go in gos) { InjectGameObject(go); }
                }
            }

            sw.Stop();
            Debug.LogFormat($"Services initialization finished, cost : {sw.ElapsedMilliseconds}ms. ");
        }

        #region Imeplementation of IContext

        public void InjectGameObject(GameObject targetGo)
        {
            var monoBehaviours = targetGo.GetComponents<MonoBehaviour>();
            foreach (var monoBehaviour in monoBehaviours)
            {
                if (monoBehaviour == null) { continue; }

                Inject(monoBehaviour);
            }
        }

        public object Create(Type type, string id = null)
        {
            return Container.Create(type, id);
        }

        public T Create<T>(string id = null) where T : class
        {
            return Container.Create<T>(id);
        }

        public object Instance(Type type, string id = null)
        {
            return Container.Instance(type, id);
        }

        public T Instance<T>(string id = null) where T : class
        {
            return Container.Instance<T>(id);
        }

        public object Singleton(Type contract, string id = null)
        {
            return Container.Singleton(contract, id);
        }

        public T Singleton<T>(string id = null) where T : class
        {
            return Container.Singleton<T>(id);
        }

        public object Resolve(Type contract, string id = null)
        {
            return Container.Resolve(contract, id);
        }

        public T Resolve<T>(string id = null) where T : class
        {
            return Container.Resolve<T>(id);
        }

        public T Inject<T>(T target)
        {
            return Container.Inject(target);
        }

        #endregion
    }
}