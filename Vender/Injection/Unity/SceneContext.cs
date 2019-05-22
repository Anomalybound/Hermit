using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Hermit.Injection
{
    [ScriptOrder(-10000)]
    public class SceneContext : MonoBehaviour, IContext
    {
        public IDependencyContainer Container { get; } = new DiContainer();

        [SerializeField]
        protected MonoModule[] Modules = { };

        protected virtual void Awake()
        {
            if (Context.GlobalContext == null) { Context.SetCurrentContext(this); }

            var sw = Stopwatch.StartNew();
            sw.Start();

            foreach (var module in Modules)
            {
                if (module == null) { continue; }

                module.RegisterBindings(Container);
            }

            Container.Build();

            foreach (var module in Modules)
            {
                if (module == null) { continue; }

                module.Initialization(Container);
            }

            foreach (var go in (GameObject[]) Resources.FindObjectsOfTypeAll(typeof(GameObject)))
            {
                if (string.IsNullOrEmpty(go.scene.name) || go.hideFlags == HideFlags.NotEditable ||
                    go.hideFlags == HideFlags.HideAndDontSave) { continue; }

                InjectGameObject(go);
            }

            sw.Stop();
            Debug.LogFormat($"Scene context setup finished, cost : {sw.ElapsedMilliseconds}ms. ");
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