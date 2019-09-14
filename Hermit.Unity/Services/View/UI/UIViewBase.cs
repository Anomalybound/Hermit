using System;
using Hermit.UIStack;

namespace Hermit.View
{
    public abstract class UIViewBase<TViewModel> : Widget, IViewModelProvider where TViewModel : ViewModel
    {
        public TViewModel DataContext { get; protected set; }

        public void SetViewModel(TViewModel dataContext)
        {
            DataContext = dataContext;
            DataReadyEvent?.Invoke();
        }

        public void SetViewModel(object context)
        {
            if (context is TViewModel viewModel) { DataContext = viewModel; }
            else { Her.Warn($"{context} is not matching {typeof(TViewModel)}"); }

            DataReadyEvent?.Invoke();
        }

        public TViewModel GetViewModel()
        {
            return DataContext;
        }

        public string GetViewModelTypeName => typeof(TViewModel).FullName;

        public void ReBindAll()
        {
            if (DataBindings != null)
            {
                foreach (var db in DataBindings)
                {
                    db.Disconnect();
                    db.SetupBinding();
                    db.Connect();
                }
            }
        }

        public event Action DataReadyEvent;

        ViewModel IViewModelProvider.GetViewModel()
        {
            return GetViewModel();
        }

        protected DataBindingBase[] DataBindings;

        protected virtual void Awake()
        {
            SetUpViewInfo();
        }

        public override void CleanUpViewInfo()
        {
            base.CleanUpViewInfo();

            if (!DataContext.Reusable && DataContext is IDisposable disposable) { disposable.Dispose(); }
        }

        public override void SetUpViewInfo()
        {
            base.SetUpViewInfo();
            DataBindings = GetComponentsInChildren<DataBindingBase>();
        }
    }

    public abstract class UIViewBase : UIViewBase<ViewModel> { }
}