using System;
using System.Collections.Generic;
using Hermit.DataBinding;
using UnityEngine;

namespace Hermit.Views
{
    public enum ViewModelProvideType
    {
        Manual,
        AutoCreate,
        AutoResolve
    }

    public sealed class ViewModelProvider : MonoBehaviour, IViewModelProvider, IView
    {
        #region ViewModel Provider

        [SerializeField] private ViewModelProvideType provideType = ViewModelProvideType.Manual;

        [SerializeField] private string viewModelTypeString;

        #endregion

        private IViewManager ViewManager { get; set; }

        public List<DataBindingBase> DataBindings { get; } = new List<DataBindingBase>();

        #region IView

        public ulong ViewId { get; private set; }

        public Component component => this;

        private void Awake()
        {
            object videModelInstance = null;

            switch (provideType)
            {
                case ViewModelProvideType.Manual:
                    break;
                case ViewModelProvideType.AutoCreate:
                    videModelInstance = App.Create(AssemblyHelper.GetTypeInAppDomain(viewModelTypeString));
                    break;
                case ViewModelProvideType.AutoResolve:
                    videModelInstance = App.Resolve(AssemblyHelper.GetTypeInAppDomain(viewModelTypeString));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (videModelInstance != null) { SetViewModel(videModelInstance); }

            SetUpViewInfo();
        }

        private void OnDestroy()
        {
            CleanUpViewInfo();
        }

        /// <summary>
        /// Should be call manually.
        /// </summary>
        public void SetUpViewInfo()
        {
            ViewManager = App.Resolve<IViewManager>();
            ViewId = ViewManager.Register(this);
        }

        /// <summary>
        /// Should be call manually.
        /// </summary>
        public void CleanUpViewInfo()
        {
            ViewManager.UnRegister(ViewId);
            if (DataContext != null && !DataContext.Reusable) { DataContext?.Dispose(); }
        }

        #endregion

        #region IViewModelProvider

        public ViewModel DataContext { get; private set; }

        public void SetViewModel(object context)
        {
            if (context is ViewModel viewModel) { DataContext = viewModel; }
            else { App.Warn($"{context} is not a ViewModel."); }

            OnDataReady?.Invoke();
            ReBindAll();
        }

        public ViewModel GetViewModel() => DataContext;

        public string ViewModelTypeName
        {
            get => viewModelTypeString;
#if UNITY_EDITOR
            set => viewModelTypeString = value;
#endif
        }

        public void ReBindAll()
        {
            if (DataBindings == null) { return; }

            foreach (var db in DataBindings)
            {
                db.Disconnect();
                db.SetupBinding();
                db.Connect();
            }
        }

        public event Action OnDataReady;

        ViewModel IViewModelProvider.GetViewModel()
        {
            return GetViewModel();
        }

        #endregion
    }
}
