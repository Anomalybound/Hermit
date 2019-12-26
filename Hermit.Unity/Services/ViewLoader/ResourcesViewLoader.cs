using System;
using System.Threading.Tasks;
using Hermit.View;
using UnityEngine;

namespace Hermit.Services
{
    public class ResourcesViewLoader : Singleton<ResourcesViewLoader>, IViewLoader
    {
        public async Task<TView> LoadView<TView>() where TView : IView
        {
            var viewInfo = ViewAttribute.Find<TView>();
            var request = Resources.LoadAsync<GameObject>(viewInfo.Path);

            while (!request.isDone) { await Task.Yield(); }

            var go = request.asset as GameObject;
            if (go == null) { throw new Exception($"Can't load view at path:{viewInfo.Path}"); }

            var view = go.GetComponent<TView>();
            if (view != null) { return view; }

            throw new Exception($"Can't found {nameof(TView)} on {go.name}");
        }
    }
}