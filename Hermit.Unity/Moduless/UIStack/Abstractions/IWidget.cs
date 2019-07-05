using System.Threading.Tasks;

namespace Hermit.UIStack
{
    public interface IWidget : IView
    {
        Task OnShow();

        Task OnHide();

        Task OnResume();

        Task OnFreeze();

        void DestroyWidget();

        void SetManagerInfo(string path, IUIStack manager, UIMessage message);
    }
}