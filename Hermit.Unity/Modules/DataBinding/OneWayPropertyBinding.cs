using System;
using System.ComponentModel;
using System.Reflection;
using Hermit.DataBinding;
using UnityEngine;

namespace Hermit
{
    public class OneWayPropertyBinding : DataBindingBase
    {
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

        protected Action<object> ViewSetter;

        protected Func<object> ViewGetter;

        protected Action<object> ViewModelSetter;

        protected Func<object> ViewModelGetter;

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
            var convertedValue = _viewAdapterOptions != null
                ? ViewAdapterInstance?.Convert(rawValue, _viewAdapterOptions)
                : ViewAdapterInstance?.Convert(rawValue);

            ViewSetter.Invoke(ViewAdapterInstance != null ? convertedValue : rawValue);
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

            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    var fieldInfo = memberInfo as FieldInfo;
                    ViewSetter = value => fieldInfo?.SetValue(component, value);
                    ViewGetter = () => fieldInfo?.GetValue(component);
                    break;
                case MemberTypes.Property:
                    var propertyInfo = memberInfo as PropertyInfo;
                    ViewSetter = value => propertyInfo?.SetValue(component, value);
                    ViewGetter = () => propertyInfo?.GetValue(component);
                    break;
                default:
                    throw new Exception(
                        $"MemberType: {memberInfo.MemberType} is not supported in one way property binding.");
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
                    ViewModelSetter = value => fieldInfo?.SetValue(ViewModel, value);
                    break;
                case MemberTypes.Property:
                    var propertyInfo = memberInfo as PropertyInfo;
                    ViewModelGetter = () => propertyInfo?.GetValue(ViewModel);
                    ViewModelSetter = value => propertyInfo?.SetValue(ViewModel, value);
                    break;
                default:
                    throw new Exception(
                        $"MemberType: {memberInfo.MemberType} is not supported in one way property binding.");
            }

            #endregion
        }
    }
}