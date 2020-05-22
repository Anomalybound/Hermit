using System;
using System.Reflection;
using Hermit.Common;
using Hermit.Injection;
using Hermit.Injection.Attributes;
using Hermit.Injection.Core;
using Hermit.Service.Events;
using Hermit.Service.Log;
using Hermit.Service.Messages;
using Hermit.Service.Stores;
using Hermit.Service.Views;
using Hermit.Service.Views.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Hermit
{
    public sealed partial class Her
    {
        public static readonly Version Version = new Version("0.5.0");

        internal static Her Current
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

        public IMessageHub MessageHub { get; }

        public ILog Logger { get; }

        public IUIStack UIStack => _uiStack ?? (_uiStack = Context.Container.Resolve<IUIStack>());

        private IUIStack _uiStack;

        private IViewManager ViewManager { get; set; }

        private HermitGeneralSettings GeneralSettings { get; set; }

        private IContext Context { get; set; }

        private Her()
        {
            Logger = UnityLog.Instance;
            EventBroker = new EventBroker(MainThreadDispatcher.Instance);
            MessageHub = new MessageHub();
        }

        ~Her() { }

        [Inject]
        internal void Injection(IViewManager viewManager, HermitGeneralSettings generalSettings)
        {
            ViewManager = viewManager;
            GeneralSettings = generalSettings;

            Context = Contexts.GlobalContext;

            // Add default error handler
            if (GeneralSettings.RegisterGlobalMessageHandler)
            {
                MessageHub.RegisterGlobalErrorHandler(DefaultErrorMessagesHandler);
            }

            // Setup stores
            if (_stores.ContainsKey("Global")) { return; }

            var store = Context.Container.Resolve<IStore>();
            store.SetStoreId("Global");
            _stores.Add("Global", store);
        }

        private void DefaultErrorMessagesHandler(Guid token, Exception exception)
        {
            Error(exception);
        }
    }
}
