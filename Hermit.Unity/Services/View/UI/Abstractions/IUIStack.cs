using System.Threading.Tasks;

namespace Hermit.View
{
    public interface IUIStack : IUIController
    {
        Task<IUIView> PushAsync<TUIView>(IViewFactory factory = null) where TUIView : IUIView;

        Task<IUIView> ModalAsync<TUIView>(IViewFactory factory = null) where TUIView : IUIView;

        Task PopAsync();
    }
}