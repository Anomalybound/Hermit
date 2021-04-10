using Hermit.Injection;
using Hermit.Service.Events;
using Hermit.Service.Log;
using Hermit.Service.Messages;
using Hermit.Service.Stores;
using Hermit.Service.ViewLoader;
using Hermit.Views;
using UnityEngine;

namespace Hermit.ServiceProvider
{
    public class HermitKernelServiceRegistry : MonoServiceRegistry
    {
        [Header("General")]
        [SerializeField] protected HermitGeneralSettings settings;

        private void Awake()
        {
            settings ??= new HermitGeneralSettings();
        }

        public override void RegisterBindings(IDependencyContainer container)
        {
            // Built-in Services
            container.Bind<IMessageHub>().To<MessageHub>().FromInstance(App.I.MessageHub);
            container.Bind<IEventBroker>().To<EventBroker>().FromInstance(App.I.EventBroker);
            container.Bind<ILog>().To<UnityLog>().FromInstance(App.I.Logger);
            container.Bind<IStore>().To<DictionaryStore>().FromInstance(DictionaryStore.Instance);

            // View
            container.Bind<IViewLoader>().To<ResourcesViewLoader>().FromInstance(ResourcesViewLoader.Instance);
            container.Bind<IViewManager>().To<ViewManager>().FromInstance(ViewManager.Instance);
            container.Bind<IViewFactory>().To<DefaultViewFactory>();

            // Essentials
            container.BindAll<App>().FromInstance(App.I);
            container.Bind<HermitGeneralSettings>().FromInstance(settings);
            container.BindInstance(container);
        }
    }
}