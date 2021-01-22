using System;
using Cysharp.Threading.Tasks;
using Hermit.Service.ViewLoader;
using Object = UnityEngine.Object;

namespace Hermit.Views
{
    public abstract class DefaultViewFactory : IViewFactory
    {
        private readonly IViewLoader _viewLoader;

        protected DefaultViewFactory()
        {
            _viewLoader = Her.Resolve<IViewLoader>();
        }

        public async UniTask<IView> CreateInstance<TView>() where TView : IView
        {
            var prefab = await _viewLoader.LoadView<TView>();
            if (prefab == null) { throw new Exception($"Load view: {nameof(TView)} failed"); }

            var instance = Object.Instantiate(prefab.gameObject).GetComponent<IView>();
            instance.SetUpViewInfo();
            return instance;
        }

        public void ReturnInstance(IView view)
        {
            view.CleanUpViewInfo();
            Object.Destroy(view.gameObject);
        }
    }
}