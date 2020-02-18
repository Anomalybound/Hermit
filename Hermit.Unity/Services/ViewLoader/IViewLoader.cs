using System.Threading.Tasks;

namespace Hermit.View
{
    public interface IViewLoader
    {
        Task<TView> LoadView<TView>() where TView : IView;
    }
}