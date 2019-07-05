using System;
using System.ComponentModel;
using System.Reflection;
using Hermit.DataBinding;
using UnityEngine;
using Component = UnityEngine.Component;

namespace Hermit
{
    public class PropertyMethodBinding : DataBindingBase
    {
        public bool ShowDeclaredMethodsOnly = true;
        
        [SerializeField]
        private string _viewModelEntry;

        [SerializeField]
        private string _viewEntry;

        [SerializeField]
        private string viewAdapterType;

        [SerializeField]
        private AdapterOptions _viewAdapterOptions;

        #region Properties

        public string ViewAdapterType
        {
            get => viewAdapterType;
            set
            {
#if UNITY_EDITOR
                if (viewAdapterType != value) { UnityEditor.EditorUtility.SetDirty(this); }
#endif
                viewAdapterType = value;
            }
        }

        public AdapterOptions AdapterOptions
        {
            get => _viewAdapterOptions;
            set => _viewAdapterOptions = value;
        }

        public string ViewModelEntry
        {
            get => _viewModelEntry;
            set
            {
#if UNITY_EDITOR
                if (_viewModelEntry != value) { UnityEditor.EditorUtility.SetDirty(this); }
#endif
                _viewModelEntry = value;
            }
        }

        public string ViewEntry
        {
            get => _viewEntry;
            set
            {
#if UNITY_EDITOR
                if (_viewEntry != value) { UnityEditor.EditorUtility.SetDirty(this); }
#endif
                _viewEntry = value;
            }
        }

        #endregion

        #region Runtime Variables

        protected string ViewModelMemberName;

        protected Func<object> ViewModelGetter;

        protected MethodInfo MethodInvoker;

        protected Component ViewComponentInstance;

        protected IAdapter ViewAdapterInstance;

        protected bool PropertyChanging;

        #endregion

        public override void SetupBinding()
        {
            base.SetupBinding();

            BindView2ViewModel();

            GetViewAdapterInstance();
        }

        public override void Connect()
        {
            if (ViewModel != null) { ViewModel.PropertyChanged += OnPropertyChanged; }

            UpdateBinding();
        }

        public override void Disconnect()
        {
            if (ViewModel != null) { ViewModel.PropertyChanged -= OnPropertyChanged; }
        }

        public override void UpdateBinding()
        {
            UpdateProperty();
        }

        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ViewModelMemberName != e.PropertyName) { return; }

            if (!PropertyChanging) { UpdateProperty(); }
        }

        protected void UpdateProperty()
        {
            var rawValue = ViewModelGetter.Invoke();
            var convertedValue = ViewAdapterInstance?.Covert(rawValue, _viewAdapterOptions);
            var parameter = new[] {ViewAdapterInstance != null ? convertedValue : rawValue};

            if (MethodInvoker.GetParameters().Length > 0) { MethodInvoker.Invoke(ViewComponentInstance, parameter); }
            else { MethodInvoker.Invoke(ViewAdapterInstance, null); }
        }

        protected void GetViewAdapterInstance()
        {
            if (string.IsNullOrEmpty(viewAdapterType)) { return; }

            ViewAdapterInstance = Her.Resolve<IAdapter>(viewAdapterType);
        }

        protected void BindView2ViewModel()
        {
            #region View 

            var (component, memberInfo) = ParseViewEntry(this, ViewEntry);
            ViewComponentInstance = component;

            switch (memberInfo.MemberType)
            {
                case MemberTypes.Method:
                    MethodInvoker = memberInfo as MethodInfo;
                    break;

                default:
                    throw new Exception(
                        $"MemberType: {memberInfo.MemberType} is not supported in property method binding.");
            }

            #endregion

            #region View Model

            memberInfo = ParseViewModelEntry(ViewModel, ViewModelEntry);

            ViewModelMemberName = memberInfo.Name;

            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    var fieldInfo = memberInfo as FieldInfo;
                    ViewModelGetter = () => fieldInfo?.GetValue(ViewModel);
                    break;
                case MemberTypes.Property:
                    var propertyInfo = memberInfo as PropertyInfo;
                    ViewModelGetter = () => propertyInfo?.GetValue(ViewModel);
                    break;
                default:
                    throw new Exception(
                        $"MemberType: {memberInfo.MemberType} is not supported in one way property binding.");
            }

            #endregion
        }
    }
}