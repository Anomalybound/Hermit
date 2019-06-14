using System.Threading.Tasks;

namespace Hermit.UIStack
{
    public interface IUIController
    {
        Task ClearPopupsAsync();

        Task ClearFixesAsync();

        Task ClearWindowsAsync();

        Task ClearAllAsync();

        Task CloseAsync(ulong widgetId, bool recycle = false);
    }
}