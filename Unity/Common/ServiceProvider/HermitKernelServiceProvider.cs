using Hermit.Injection;
using Hermit.Injection.Core;
using Hermit.Service.Events;
using Hermit.Service.Log;
using Hermit.Service.Stores;
using Hermit.Service.Time;
using Hermit.Service.ViewLoader;
using Hermit.Service.Views;
using UnityEngine;

namespace Hermit.Common
{
    public class HermitKernelServiceProvider : MonoServiceProvider
    {
        [Header("General")]
        [SerializeField] protected HermitGeneralSettings settings;

        public override void RegisterBindings(IDependencyContainer container)
        {
            // Built-in Services
            container.Bind<IEventBroker>().To<EventBroker>().FromInstance(Her.Current.EventBroker);
            container.Bind<ILog>().To<UnityLog>().FromInstance(Her.Current.Logger);
            container.Bind<ITime>().To<UnityTime>().FromInstance(UnityTime.Instance);
            container.Bind<IStore>().To<DictionaryStore>().FromInstance(DictionaryStore.Instance);

            // View
            container.Bind<IViewLoader>().To<ResourcesViewLoader>().FromInstance(ResourcesViewLoader.Instance);
            container.Bind<IViewManager>().To<ViewManager>().FromInstance(ViewManager.Instance);
            container.Bind<IViewFactory>().To<DefaultViewFactory>();

            // Essentials
            container.BindAll<Her>().FromInstance(Her.Current);
            container.Bind<HermitGeneralSettings>().FromInstance(settings);
            container.BindInstance(container);
        }
    }
}