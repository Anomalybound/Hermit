using System;
using System.Reflection;
using Hermit.Common;
using Hermit.Injection;
using Hermit.Services;
using Hermit.UIStack;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Hermit
{
    public sealed partial class Her
    {
        public static readonly Version Version = new Version("0.3.1");

        public static Her Current
        {
            get
            {
                if (current != null) { return current; }

                current = new Her();
                var kernel = Object.FindObjectOfType<HermitKernel>();
                if (kernel != null) { return current; }

                Log("HermitKernel not found, add one to this scene.");
                var kernelObj = new GameObject("Hermit Kernel");

                var array = new MonoServiceProvider[]
                {
                    kernelObj.AddComponent<HermitKernelServiceProvider>(),
                    kernelObj.AddComponent<HermitDataBindingServiceProvider>()
                };
                kernel = kernelObj.AddComponent<HermitKernel>();

                var fieldInfo = typeof(HermitKernel).GetField("ServiceProviders"
                    , BindingFlags.NonPublic | BindingFlags.Instance);
                fieldInfo?.SetValue(kernel, array);

                return current;
            }
        }

        private static Her current;

        public IEventBroker EventBroker { get; }

        public ILog Logger { get; }

        public IUIStack UiStack => _uiStack ?? (_uiStack = _container.Resolve<IUIStack>());

        private IUIStack _uiStack { get; set; }

        private IDependencyContainer _container { get; set; }

        private IViewManager _viewManager { get; set; }

        public Her()
        {
            Logger = UnityLog.Instance;
            EventBroker = Services.EventBroker.Instance;
        }

        [Inject]
        public void Injection(IDependencyContainer container, IViewManager viewManager)
        {
            _container = container;
            _viewManager = viewManager;

            // Setup stores
            var store = container.Resolve<IStore>();
            store.SetStoreId("Global");
            stores.Add("Global", store);
        }
    }
}