using System;
using System.Threading.Tasks;

namespace Hermit.UIStack
{
    public interface IUIStack : IUIController
    {
        Task<int> Push(string widgetName);

        Task<int> Push(string widgetName, UIMessage message);

        Task<int> Push<TWidget>(string widgetName) where TWidget : Widget;

        Task<int> Push<TWidget>(string widgetName, UIMessage message) where TWidget : Widget;

        Task Pop(bool recycle = false);

        Task Pop(Action onDone, bool recycle = false);
    }
}