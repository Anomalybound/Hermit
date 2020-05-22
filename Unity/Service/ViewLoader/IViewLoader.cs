using System.Threading.Tasks;
using Hermit.Service.Views;

namespace Hermit.Service.ViewLoader
{
    public interface IViewLoader
    {
        Task<TView> LoadView<TView>() where TView : IView;
    }
}