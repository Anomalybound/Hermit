using System.Threading.Tasks;
using UnityEngine;

namespace Hermit.Services
{
    public class ResourcesViewLoader : IViewLoader
    {
        private ILog Logger { get; }

        public ResourcesViewLoader(ILog logger)
        {
            Logger = logger;
        }

        public async Task<GameObject> LoadView(string key)
        {
            var request = Resources.LoadAsync<GameObject>(key);

            await request;

            await Task.Delay(10);

            var go = request.asset as GameObject;
            if (go != null) { return go; }

            Logger.Warn($"Can't load view at key:{key}", go);
            return null;
        }

        public async Task<TView> LoadView<TView>(string key) where TView : IView
        {
            var request = Resources.LoadAsync<GameObject>(key);
            
            await request;

            await Task.Delay(10);

            var go = request.asset as GameObject;
            if (go != null)
            {
                var view = go.GetComponent<TView>();
                if (view != null) { return view; }
            }

            Logger.Warn($"Can't load view at key:{key}", go);
            return default;
        }
    }
}