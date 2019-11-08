using Hermit.Injection;
using Hermit.Services;
using UnityEngine;
using Hermit.UIStack;

namespace Hermit.Common
{
    public class HermitKernelServiceProvider : MonoServiceProvider
    {
        [Header("UI")]
        [SerializeField]
        protected UIManagerSettings uiManagerSettings = new UIManagerSettings();

        public override void RegisterBindings(IDependencyContainer container)
        {
            // Built-in Services
            container.Bind<IEventBroker>().To<EventBroker>().FromInstance(Her.Current.EventBroker);
            container.Bind<ILog>().To<UnityLog>().FromInstance(Her.Current.Logger);
            container.Bind<ITime>().To<UnityTime>().FromInstance(UnityTime.Instance);
            container.Bind<IStore>().To<DictionaryStore>().FromInstance(DictionaryStore.Instance);
            container.Bind<IUIStack>().FromMethod(BuildUIStackInstance);

            // View
            container.Bind<IViewLoader>().To<ResourcesViewLoader>().FromInstance(ResourcesViewLoader.Instance);
            container.Bind<IViewManager>().To<ViewManager>().FromInstance(ViewManager.Instance);

            // Essentials
            container.BindAll<Her>().FromInstance(Her.Current);
            container.BindInstance(container);
        }

        protected IUIStack BuildUIStackInstance(IDependencyContainer container)
        {
            return UIStackManager.BuildHierarchy(uiManagerSettings);
        }
    }
}