using System;
using System.Collections.Generic;
using System.Linq;

namespace Hermit
{
    public sealed class ViewManager : IViewManager
    {
        private readonly Dictionary<ulong, IView> ViewCaches = new Dictionary<ulong, IView>();

        private ulong ViewIdCounter { get; set; }

        public IView GetView(ulong id)
        {
            return ViewCaches.TryGetValue(id, out var view) ? view : default;
        }

        public TView GetView<TView>(ulong id) where TView : IView
        {
            if (!ViewCaches.TryGetValue(id, out var view)) { return default; }

            if (view is TView targetView) { return targetView; }

            return default;
        }

        public ulong Register<TView>(TView view) where TView : IView
        {
            var viewId = ViewIdCounter;
            ViewCaches.Add(viewId, view);

            ViewIdCounter++;
            return viewId;
        }

        public void UnRegister(ulong id)
        {
            if (!ViewCaches.ContainsKey(id))
            {
                throw new IndexOutOfRangeException(
                    $"Failed to unregister view of id: {id}, view caches contains: {string.Join("-", ViewCaches.Select(v => v.Key.ToString()))}.");
            }

            ViewCaches.Remove(id);
        }
    }
}