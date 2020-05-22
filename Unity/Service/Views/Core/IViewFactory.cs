using System.Threading.Tasks;

namespace Hermit.Service.Views
{
    public interface IViewFactory
    {
        Task<IView> CreateInstance<TView>() where TView : IView;

        void ReturnInstance(IView view);
    }
}