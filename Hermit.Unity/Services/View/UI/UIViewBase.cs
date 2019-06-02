using Hermit.UIStack;

namespace Hermit.View
{
    public abstract class UIViewBase<TViewModel> : Widget, IView where TViewModel : ViewModel
    {
        public TViewModel DataContext { get; protected set; }

        public virtual void SetViewModel(object context)
        {
            if (context is TViewModel viewModel) { DataContext = viewModel; }
            else { Her.Warn($"{context} is not matching {typeof(TViewModel)}"); }
        }

        public virtual TViewModel GetViewModel()
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

        ViewModel IViewModelProvider.GetViewModel()
        {
            return GetViewModel();
        }

        protected DataBindingBase[] DataBindings;

        protected virtual void Awake()
        {
            DataBindings = GetComponentsInChildren<DataBindingBase>();
        }
    }

    public abstract class UIViewBase : UIViewBase<EmptyViewModel>
    {
        public override void SetViewModel(object context)
        {
            DataContext = (EmptyViewModel) ViewModel.Empty;
            if (context != null) { Her.Log($"{GetType().Name} will not receive any models."); }
        }
    }
}