using System.Threading.Tasks;

namespace Hermit.UIStack
{
    public interface IUIController
    {
        Task ClearPopups();

        Task ClearFixes();

        Task ClearWindows();

        Task ClearAll();

        Task Close(int widgetId, bool recycle = false);
    }
}