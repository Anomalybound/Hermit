using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Hermit.Services
{
    public class ResourcesViewLoader : Singleton<ResourcesViewLoader>, IViewLoader
    {
        public async Task<GameObject> LoadView(string key)
        {
            var request = Resources.LoadAsync<GameObject>(key);

            while (!request.isDone) { await Task.Delay(10); }

            var go = request.asset as GameObject;
            if (go != null) { return go; }

            throw new Exception($"Can't load view at key:{key}");
        }

        public async Task<TView> LoadView<TView>(string key) where TView : IView
        {
            var request = Resources.LoadAsync<GameObject>(key);

            while (!request.isDone) { await Task.Delay(10); }

            var go = request.asset as GameObject;
            if (go != null)
            {
                var view = go.GetComponent<TView>();
                if (view != null) { return view; }
            }

            throw new Exception($"Can't load view at key:{key}");
        }
    }
}