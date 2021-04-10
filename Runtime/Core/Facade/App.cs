using System;
using System.Reflection;
using Hermit.Injection;
using Hermit.Injection.Attributes;
using Hermit.Service.Events;
using Hermit.Service.Log;
using Hermit.Service.Messages;
using Hermit.Service.Stores;
using Hermit.ServiceProvider;
using Hermit.Views;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Hermit
{
    public sealed partial class App
    {
        public static readonly Version Version = new Version("0.6.1");

        internal static App I
        {
            get
            {
                if (current != null) { return current; }

                current = new App();
                var kernel = Object.FindObjectOfType<HermitKernel>();
                if (!kernel)
                {
                    Log("HermitKernel not found, add one to this scene.");
                    var kernelObj = new GameObject("Hermit Kernel");
                    var kernelServiceRegistry = kernelObj.AddComponent<HermitKernelServiceRegistry>();

                    var array = new MonoServiceRegistry[]
                    {
                        kernelServiceRegistry,
                    };
                    kernel = kernelObj.AddComponent<HermitKernel>();

                    var fieldInfo = typeof(HermitKernel).GetField("ServiceProviders"
                        , BindingFlags.NonPublic | BindingFlags.Instance);
                    fieldInfo?.SetValue(kernel, array);
                }

                current.RootContainer = kernel.Container;
                return current;
            }
        }

        private static App current;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            current = null;
        }

        public IEventBroker EventBroker { get; }

        public IMessageHub MessageHub { get; }

        public ILog Logger { get; }

        private IViewManager ViewManager { get; set; }

        private HermitGeneralSettings GeneralSettings { get; set; }

        public IDependencyContainer RootContainer { get; private set; }

        private App()
        {
            Logger = UnityLog.Instance;
            EventBroker = new EventBroker(MainThreadDispatcher.Instance);
            MessageHub = new MessageHub();
        }

        [Inject]
        internal void Injection(IViewManager viewManager, HermitGeneralSettings generalSettings, IStore store)
        {
            ViewManager = viewManager;
            GeneralSettings = generalSettings;

            // Add default error handler
            if (GeneralSettings.RegisterGlobalMessageHandler)
            {
                MessageHub.RegisterGlobalErrorHandler(DefaultErrorMessagesHandler);
            }

            // Setup stores
            StoreId = "Global";
            if (_stores.ContainsKey(StoreId)) { return; }

            store.SetStoreId(StoreId);
            _stores.Add(StoreId, store);
        }

        private static void DefaultErrorMessagesHandler(Guid token, Exception exception)
        {
            Error(exception);
        }
    }
}
