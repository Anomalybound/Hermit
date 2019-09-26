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

        public override void RegisterBindings(IDependencyContainer Container)
        {
            // Built-in Services
            Container.Bind<IEventBroker>().To<EventBroker>().FromInstance(Her.Current.EventBroker);
            Container.Bind<ILog>().To<UnityLog>().FromInstance(Her.Current.Logger);
            Container.Bind<ITime>().To<UnityTime>().FromInstance(UnityTime.Instance);
            Container.Bind<IStore>().To<DictionaryStore>().FromInstance(DictionaryStore.Instance);
            Container.Bind<IUIStack>().FromMethod(BuildUIStackInstance);

            // View
            Container.Bind<IViewLoader>().To<ResourcesViewLoader>().FromInstance(ResourcesViewLoader.Instance);
            Container.Bind<IViewManager>().To<ViewManager>().FromInstance(ViewManager.Instance);

            // Essentials
            Container.BindAll<Her>().FromInstance(Her.Current);
            Container.BindInstance(Container);
        }

        protected IUIStack BuildUIStackInstance(IDependencyContainer container)
        {
            return UIStackManager.BuildHierarchy(uiManagerSettings);
        }
    }
}