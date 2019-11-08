using System;
using System.ComponentModel;
using System.Reflection;
using Hermit.DataBinding;
using UnityEngine;
using Component = UnityEngine.Component;

namespace Hermit
{
    [AddComponentMenu("Hermit/Data Binding/Method Binding")]
    public class MethodBinding : DataBindingBase
    {
        public bool showDeclaredMethodsOnly = true;

        [SerializeField] private string viewModelEntry;

        [SerializeField] private string viewEntry;

        [SerializeField] private string viewAdapterType;

        [SerializeField] private AdapterOptions viewAdapterOptions;

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
            get => viewAdapterOptions;
            set => viewAdapterOptions = value;
        }

        public string ViewModelEntry
        {
            get => viewModelEntry;
            set
            {
#if UNITY_EDITOR
                if (_viewModelEntry != value) { UnityEditor.EditorUtility.SetDirty(this); }
#endif
                viewModelEntry = value;
            }
        }

        public string ViewEntry
        {
            get => viewEntry;
            set
            {
#if UNITY_EDITOR
                if (_viewEntry != value) { UnityEditor.EditorUtility.SetDirty(this); }
#endif
                viewEntry = value;
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
            base.Connect();

            if (ViewModel != null) { ViewModel.PropertyChanged += OnPropertyChanged; }

            UpdateBinding();
        }

        public override void Disconnect()
        {
            base.Disconnect();

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
            var convertedValue = viewAdapterOptions != null
                ? ViewAdapterInstance?.Convert(rawValue, viewAdapterOptions)
                : ViewAdapterInstance?.Convert(rawValue);

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
                    throw new Exception($"MemberType: {memberInfo.MemberType} is not supported in method binding.");
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
                    throw new Exception($"MemberType: {memberInfo.MemberType} is not supported in method binding.");
            }

            #endregion
        }
    }
}