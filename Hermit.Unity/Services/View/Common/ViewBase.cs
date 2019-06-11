using System;
using UnityEngine;

namespace Hermit
{
    public abstract class ViewBase<TViewModel> : MonoBehaviour, IView where TViewModel : ViewModel
    {
        public TViewModel DataContext { get; protected set; }

        public virtual void SetViewModel(object context)
        {
            if (context is TViewModel viewModel) { DataContext = viewModel; }
            else { Her.Warn($"{context} is not matching {typeof(TViewModel)}"); }

            DataReadyEvent?.Invoke();
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

        public event Action DataReadyEvent;

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

    public abstract class ViewBase : ViewBase<EmptyViewModel>
    {
        public override void SetViewModel(object context)
        {
            DataContext = (EmptyViewModel) ViewModel.Empty;
            if (context != null) { Her.Log($"{GetType().Name} will not receive any models."); }
        }
    }
}