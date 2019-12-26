using System.Threading.Tasks;

namespace Hermit.View
{
    public interface IUIController
    {
        void RegisterDefaultFactory(IViewFactory factory);

        Task CloseAsync(ulong viewId);
    }
}