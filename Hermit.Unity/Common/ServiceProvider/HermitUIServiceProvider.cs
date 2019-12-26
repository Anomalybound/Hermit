using Hermit.Injection;
using Hermit.View;
using UnityEngine;

namespace Hermit.Common
{
    public class HermitUIServiceProvider : MonoServiceProvider
    {
        [Header("UI")]
        [SerializeField] protected UIControllerSettings uiControllerSettings = new UIControllerSettings();

        public override void RegisterBindings(IDependencyContainer container)
        {
            container.Bind<IUIStack>().FromMethod(BuildUIStackInstance).NonLazy();
        }

        protected IUIStack BuildUIStackInstance(IDependencyContainer container)
        {
            return UIController.Build(uiControllerSettings);
        }
    }
}