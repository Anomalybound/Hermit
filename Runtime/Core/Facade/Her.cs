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
    public sealed partial class Her
    {
        public static readonly Version Version = new Version("0.6.0");

        internal static Her I
        {
            get
            {
                if (current != null) { return current; }

                current = new Her();
                var kernel = Object.FindObjectOfType<HermitKernel>();
                if (kernel != null) { return current; }

                Log("HermitKernel not found, add one to this scene.");
                var kernelObj = new GameObject("Hermit Kernel");
                var kernelServiceProvider = kernelObj.AddComponent<HermitKernelServiceRegistry>();

                var array = new MonoServiceRegistry[]
                {
                    kernelServiceProvider
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

        private IViewManager ViewManager { get; set; }

        private HermitGeneralSettings GeneralSettings { get; set; }

        private IContext Context { get; set; }

        private Her()
        {
            Logger = UnityLog.Instance;
            EventBroker = new EventBroker(MainThreadDispatcher.Instance);
            MessageHub = new MessageHub();
        }

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
            StoreId = "Global";
            if (_stores.ContainsKey(StoreId)) { return; }

            var store = Context.Container.Resolve<IStore>();
            store.SetStoreId(StoreId);
            _stores.Add(StoreId, store);
        }

        private static void DefaultErrorMessagesHandler(Guid token, Exception exception)
        {
            Error(exception);
        }
    }
}
