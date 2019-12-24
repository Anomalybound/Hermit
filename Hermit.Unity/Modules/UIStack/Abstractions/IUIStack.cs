﻿using System.Threading.Tasks;

namespace Hermit.UIStack
{
    public interface IUIStack : IUIController
    {
        Task<ulong> PushAsync(string widgetName, IWidgetFactory factory = null);

        Task<ulong> PushAsync<TWidget>(string widgetName, IWidgetFactory factory = null) where TWidget : Widget;

        Task PopAsync(bool reuse = false);
    }
}