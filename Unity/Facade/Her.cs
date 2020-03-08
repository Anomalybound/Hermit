using System;
using System.Reflection;
using Hermit.Common;
using Hermit.Injection;
using Hermit.Services;
using Hermit.View;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Hermit
{
    public sealed partial class Her
    {
        public static readonly Version Version = new Version("0.4.5");

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
                var kernelServiceProvider = kernelObj.AddComponent<HermitKernelServiceProvider>();
                var uiServiceProvider = kernelObj.AddComponent<HermitUIServiceProvider>();

                // TODO: Temporary workaround
                uiServiceProvider.uiRootPrefab = Resources.Load<UIRoot>("UI Root");

                var array = new MonoServiceProvider[]
                {
                    kernelServiceProvider, uiServiceProvider
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

        public IUIStack UIStack => _uiStack ?? (_uiStack = Context.Container.Resolve<IUIStack>());

        private IUIStack _uiStack;

        private IViewManager ViewManager { get; set; }

        private HermitGeneralSettings GeneralSettings { get; set; }

        private IContext Context { get; set; }

        public Her()
        {
            Logger = UnityLog.Instance;
            EventBroker = Hermit.EventBroker.Default;
        }

        [Inject]
        public void Injection(IViewManager viewManager, HermitGeneralSettings generalSettings)
        {
            ViewManager = viewManager;
            GeneralSettings = generalSettings;
            
            Context = Contexts.GlobalContext;

            // Setup stores
            var store = Context.Container.Resolve<IStore>();
            store.SetStoreId("Global");
            _stores.Add("Global", store);
        }
    }
}
