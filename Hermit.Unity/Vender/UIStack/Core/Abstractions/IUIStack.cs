using System;
using System.Threading.Tasks;

namespace Hermit.UIStack
{
    public interface IUIStack : IUIController
    {
        Task<int> PushAsync(string widgetName);

        Task<int> PushAsync(string widgetName, UIMessage message);

        Task<int> PushAsync<TWidget>(string widgetName) where TWidget : Widget;

        Task<int> PushAsync<TWidget>(string widgetName, UIMessage message) where TWidget : Widget;

        Task PopAsync(bool recycle = false);

        Task PopAsync(Action onDone, bool recycle = false);
    }
}