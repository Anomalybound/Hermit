using System.Threading.Tasks;

namespace Hermit.UIStack
{
    public interface IUIController
    {
        void RegisterDefaultFactory(IWidgetFactory factory);

        Task ClearPopupsAsync(bool reuse = false);

        Task ClearWindowsAsync(bool reuse = false);

        Task ClearAllAsync(bool reuse = false);

        Task CloseAsync(ulong widgetId, bool reuse = false);
    }
}