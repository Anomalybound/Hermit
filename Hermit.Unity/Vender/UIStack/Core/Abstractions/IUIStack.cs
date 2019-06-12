using System;
using System.Threading.Tasks;

namespace Hermit.UIStack
{
    public interface IUIStack : IUIController
    {
        Task<ulong> PushAsync(string widgetName);

        Task<ulong> PushAsync(string widgetName, UIMessage message);

        Task<ulong> PushAsync<TWidget>(string widgetName) where TWidget : Widget;

        Task<ulong> PushAsync<TWidget>(string widgetName, UIMessage message) where TWidget : Widget;

        Task PopAsync(bool recycle = false);

        Task PopAsync(Action onDone, bool recycle = false);
    }
}