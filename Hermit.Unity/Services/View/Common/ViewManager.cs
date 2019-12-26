using System;
using System.Collections.Generic;
using System.Linq;

namespace Hermit.View
{
    public sealed class ViewManager : Singleton<ViewManager>, IViewManager
    {
        private readonly Dictionary<ulong, IView> _viewCaches = new Dictionary<ulong, IView>();

        private ulong ViewIdCounter { get; set; }

        public IView GetView(ulong id)
        {
            return _viewCaches.TryGetValue(id, out var view) ? view : default;
        }

        public TView GetView<TView>(ulong id) where TView : IView
        {
            if (!_viewCaches.TryGetValue(id, out var view)) { return default; }

            if (view is TView targetView) { return targetView; }

            return default;
        }

        public ulong Register<TView>(TView view) where TView : IView
        {
            var viewId = ViewIdCounter;
            _viewCaches.Add(viewId, view);

            ViewIdCounter++;
            return viewId;
        }

        public void UnRegister(ulong id)
        {
            if (!_viewCaches.ContainsKey(id))
            {
                throw new IndexOutOfRangeException(
                    $"Failed to unregister view of id: {id}, view caches contains: {string.Join("-", _viewCaches.Select(v => v.Key.ToString()))}.");
            }

            _viewCaches.Remove(id);
        }
    }
}