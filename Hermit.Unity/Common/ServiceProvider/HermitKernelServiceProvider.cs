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
        protected bool IsLandscape = true;

        [SerializeField]
        protected Vector2 ReferenceResolution = new Vector2(1920, 1080);

        [SerializeField]
        [Tooltip("Can left empty.")]
        protected UIStackManager UiStackInstance;

        public override void RegisterBindings(IDependencyContainer Container)
        {
            // Built-in Services
            Container.Bind<IEventBroker>().To<EventBroker>().FromInstance(EventBroker.Current);
            Container.Bind<ITime>().To<UnityTime>();
            Container.Bind<ILog>().To<UnityLog>().FromInstance(new UnityLog(EnableLog));
            Container.Bind<IUIStack>().FromMethod(BuildUIStackInstance);
            Container.Bind<IStore>().To<DictionaryStore>();

            // View Loader
            Container.Bind<IViewLoader>().To<ResourcesViewLoader>();

            // Essentials
            Container.BindInstance(Container);
            Container.BindAll<Her>();

            // MonoBehaviors
            Container.Bind<EngineRunner>().FromMethod(EngineRunner.CreateInstance);
        }

        protected IUIStack BuildUIStackInstance(IDependencyContainer container)
        {
            return UiStackInstance != null
                ? UIStackManager.FromInstance(UiStackInstance)
                : UIStackManager.BuildHierarchy(IsLandscape, ReferenceResolution);
        }
    }
}