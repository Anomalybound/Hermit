using System.Threading.Tasks;

namespace Hermit.Service.Views.UI
{
    public interface IUIController
    {
        void RegisterDefaultFactory(IViewFactory factory);

        Task CloseAsync(ulong viewId);
    }
}