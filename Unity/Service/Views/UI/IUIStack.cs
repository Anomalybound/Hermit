using System.Threading.Tasks;

namespace Hermit.Service.Views.UI
{
    public interface IUIStack : IUIController
    {
        Task<IUIView> PushAsync<TUIView>(IViewFactory factory = null) where TUIView : IUIView;

        Task<IUIView> ModalAsync<TUIView>(IViewFactory factory = null) where TUIView : IUIView;

        Task PopAsync();
    }
}