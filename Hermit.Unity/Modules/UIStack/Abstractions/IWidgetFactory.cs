﻿using System.Threading.Tasks;

namespace Hermit.UIStack
{
    public interface IWidgetFactory
    {
         Task<Widget> CreateInstance(IUIStack manager, string name);

        void ReturnInstance(Widget widget);
    }
}