﻿using System;
using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace Hermit.UIStack
{
    public class DefaultWidgetFactory : IWidgetFactory
    {
        public async Task<Widget> CreateInstance(IUIStack manager, string name)
        {
            var loader = Her.Resolve<IViewLoader>();
            var prefab = await loader.LoadView(name);
            if (prefab == null) { throw new Exception($"Load view: {name} failed"); }

            var instance = Object.Instantiate(prefab).GetComponent<Widget>();
            instance.SetUpWidgetInfo(name, manager);
            return instance;
        }

        public void ReturnInstance(Widget widget)
        {
            widget.CleanUpViewInfo();
            Object.Destroy(widget.gameObject);
        }
    }
}