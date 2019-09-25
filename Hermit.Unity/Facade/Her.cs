using System;
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
        public static readonly Version Version = new Version("0.3.0");

        public static Her Current
        {
            get
            {
                if (current == null)
                {
                    current = new Her();
                    var kernel = Object.FindObjectOfType<HermitKernel>();
                    if (kernel != null) { return current; }

                    Log("HermitKernel not found, add one to this scene.");
                    var kernelObj = new GameObject("Hermit Kernel");
                    kernelObj.AddComponent<HermitKernel>();
                }

                return current;
            }
        }

        private static Her current;

        public IEventBroker EventBroker { get; }

        public ILog Logger { get; }

        private IDependencyContainer _container { get; set; }

        private IUIStack _uiStack { get; set; }

        private IViewManager _viewManager { get; set; }

        public Her()
        {
            Logger = UnityLog.Instance;
            EventBroker = Services.EventBroker.Instance;

            current = this;
        }

        [Inject]
        public void Injection(IDependencyContainer container, IUIStack uiStack, IViewManager viewManager)
        {
            _container = container;
            _uiStack = uiStack;
            _viewManager = viewManager;

            // Setup stores
            var store = container.Resolve<IStore>();
            store.SetStoreId("Global");
            stores.Add("Global", store);
        }
    }
}