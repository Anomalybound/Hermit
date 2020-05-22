using System.Threading.Tasks;

namespace Hermit.Service.Views.UI
{
    public abstract class UIViewBase<TViewModel> : ViewBase<TViewModel> where TViewModel : ViewModel
    {
        protected IUIController UIController { get; private set; }

        #region IUIView

        public virtual async Task OnShow()
        {
            await Task.CompletedTask;
        }

        public virtual async Task OnHide()
        {
            await Task.CompletedTask;
        }

        public virtual async Task OnResume()
        {
            await Task.CompletedTask;
        }

        public virtual async Task OnFreeze()
        {
            await Task.CompletedTask;
        }

        public async void Close()
        {
            await UIController.CloseAsync(ViewId);
        }

        #endregion

        public override void SetUpViewInfo()
        {
            base.SetUpViewInfo();

            UIController = Her.Resolve<IUIController>();
        }
    }
}