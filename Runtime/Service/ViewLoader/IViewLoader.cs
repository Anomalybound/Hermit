using Cysharp.Threading.Tasks;
using Hermit.Views;

namespace Hermit.Service.ViewLoader
{
    public interface IViewLoader
    {
        UniTask<TView> LoadView<TView>() where TView : IView;
    }
}