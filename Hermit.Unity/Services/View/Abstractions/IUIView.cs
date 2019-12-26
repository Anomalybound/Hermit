using System.Threading.Tasks;

namespace Hermit.View
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