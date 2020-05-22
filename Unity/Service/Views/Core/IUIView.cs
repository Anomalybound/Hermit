using System.Threading.Tasks;

namespace Hermit.Service.Views
{
    public interface IUIView : IView
    {
        Task OnShow();

        Task OnHide();

        Task OnResume();

        Task OnFreeze();

        void Close();
    }
}