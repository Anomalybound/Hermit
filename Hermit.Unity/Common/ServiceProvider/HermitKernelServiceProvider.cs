using Hermit.Injection;
using Hermit.Services;
using UnityEngine;
using Hermit.UIStack;

namespace Hermit.Common
{
    public class HermitKernelServiceProvider : MonoServiceProvider
    {
        [Header("General")]
        [SerializeField]
        protected bool EnableLog = true;

        [Header("UI")]
        [SerializeField]
        protected UIManagerSettings uiManagerSettings;

        public override void RegisterBindings(IDependencyContainer Container)
        {
            // Built-in Services
            Container.Bind<IEventBroker>().To<EventBroker>().FromInstance(EventBroker.Current);
            Container.Bind<ITime>().To<UnityTime>();
            Container.Bind<ILog>().To<UnityLog>().FromInstance(new UnityLog(EnableLog));
            Container.Bind<IUIStack>().FromMethod(BuildUIStackInstance);
            Container.BindAll<DictionaryStore>();

            // View
            Container.Bind<IViewLoader>().To<ResourcesViewLoader>();
            Container.Bind<IViewManager>().To<ViewManager>();

            // Essentials
            Container.BindInstance(Container);
            Container.BindAll<Her>();

            // MonoBehaviors
            Container.Bind<EngineRunner>().FromMethod(EngineRunner.CreateInstance);
        }

        protected IUIStack BuildUIStackInstance(IDependencyContainer container)
        {
            return UIStackManager.BuildHierarchy(uiManagerSettings);
        }
    }
}