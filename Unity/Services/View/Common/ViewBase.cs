using System;
using UnityEngine;

namespace Hermit.View
{
    public abstract class ViewBase<TViewModel> : MonoBehaviour, IViewModelProvider, IView where TViewModel : ViewModel
    {
        protected IViewManager ViewManager { get; private set; }

        protected DataBindingBase[] DataBindings;

        #region IView

        public ulong ViewId { get; private set; }

        public Component component => this;

        /// <summary>
        /// Should be call manually.
        /// </summary>
        public virtual void SetUpViewInfo()
        {
            ViewManager = Her.Resolve<IViewManager>();
            ViewId = ViewManager.Register(this);

            DataBindings = GetComponentsInChildren<DataBindingBase>();
        }

        /// <summary>
        /// Should be call manually.
        /// </summary>
        public virtual void CleanUpViewInfo()
        {
            ViewManager.UnRegister(ViewId);
            if (DataContext != null && !DataContext.Reusable) { DataContext?.Dispose(); }
        }

        #endregion

        #region IViewModelProvider

        public TViewModel DataContext { get; protected set; }

        public void SetViewModel(TViewModel dataContext)
        {
            DataContext = dataContext;
            OnDataReady?.Invoke();
        }

        public void SetViewModel(object context)
        {
            if (context is TViewModel viewModel) { DataContext = viewModel; }
            else { Her.Warn($"{context} is not matching {typeof(TViewModel)}"); }

            OnDataReady?.Invoke();
            ReBindAll();
        }

        public virtual TViewModel GetViewModel() => DataContext;

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

        public event Action OnDataReady;

        ViewModel IViewModelProvider.GetViewModel()
        {
            return GetViewModel();
        }

        #endregion
    }

    public abstract class ViewBase : ViewBase<ViewModel> { }
}