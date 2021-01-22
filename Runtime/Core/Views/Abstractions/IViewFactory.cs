using Cysharp.Threading.Tasks;

namespace Hermit.Views
{
    public interface IViewFactory
    {
        UniTask<IView> CreateInstance<TView>() where TView : IView;

        void ReturnInstance(IView view);
    }
}