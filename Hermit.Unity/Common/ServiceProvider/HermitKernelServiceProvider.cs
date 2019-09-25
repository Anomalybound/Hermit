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
        protected UIManagerSettings uiManagerSettings;

        public override void RegisterBindings(IDependencyContainer Container)
        {
            // Built-in Services
            Container.Bind<IEventBroker>().To<EventBroker>().FromInstance(Her.Current.EventBroker);
            Container.Bind<ILog>().To<UnityLog>().FromInstance(Her.Current.Logger);
            Container.Bind<ITime>().To<UnityTime>().FromInstance(UnityTime.Instance);
            Container.Bind<IUIStack>().FromMethod(BuildUIStackInstance);
            Container.Bind<IStore>().To<DictionaryStore>().FromInstance(DictionaryStore.Instance);

            // View
            Container.Bind<IViewLoader>().To<ResourcesViewLoader>();
            Container.Bind<IViewManager>().To<ViewManager>();

            // Essentials
            Container.BindInstance(Container);
            Container.BindAll<Her>();
        }

        protected IUIStack BuildUIStackInstance(IDependencyContainer container)
        {
            return UIStackManager.BuildHierarchy(uiManagerSettings);
        }
    }
}