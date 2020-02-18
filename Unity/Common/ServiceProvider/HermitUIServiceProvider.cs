using System;
using Hermit.Injection;
using Hermit.View;
using UnityEngine;

namespace Hermit.Common
{
    public class HermitUIServiceProvider : MonoServiceProvider
    {
        public UIRoot uiRootPrefab;

        public override void RegisterBindings(IDependencyContainer container)
        {
            container.Bind<IUIStack>().FromMethod(BuildUIStackInstance).NonLazy();
        }

        protected IUIStack BuildUIStackInstance(IDependencyContainer container)
        {
            if (uiRootPrefab == null) { throw new Exception("Please assign ui root prefab."); }

            return Instantiate(uiRootPrefab);
        }
    }
}
