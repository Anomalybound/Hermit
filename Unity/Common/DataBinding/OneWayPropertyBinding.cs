using System;
using System.ComponentModel;
using System.Reflection;
using Hermit.Common.DataBinding.Core;
using Hermit.Service.Views;
using UnityEngine;

namespace Hermit.Common.DataBinding
{
    [AddComponentMenu("Hermit/Data Binding/One-way Binding")]
    public class OneWayPropertyBinding : DataBindingBase
    {
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
                if (viewModelEntry != value) { UnityEditor.EditorUtility.SetDirty(this); }
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
                if (viewEntry != value) { UnityEditor.EditorUtility.SetDirty(this); }
#endif
                viewEntry = value;
            }
        }

        #endregion

        #region Runtime Variables

        protected string ViewModelMemberName;

        protected Action<object, object> ViewSetter;

        protected Func<object, object> ViewGetter;

        protected Action<object, object> ViewModelSetter;

        protected Func<object, object> ViewModelGetter;

        protected IAdapter ViewAdapterInstance;

        protected bool PropertyChanging;

        protected UnityEngine.Component ComponentInstance;
        protected ViewModel ViewModelInstance;

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
            var rawValue = ViewModelGetter.Invoke(ViewModelInstance);
            var convertedValue = viewAdapterOptions != null
                ? ViewAdapterInstance?.Convert(rawValue, viewAdapterOptions)
                : ViewAdapterInstance?.Convert(rawValue);

            ViewSetter.Invoke(ComponentInstance, ViewAdapterInstance != null ? convertedValue : rawValue);
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
            ComponentInstance = component;
            
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    var fieldInfo = memberInfo as FieldInfo;
//                    ViewSetter = value => fieldInfo?.SetValue(component, value);
//                    ViewGetter = () => fieldInfo?.GetValue(component);

                    ViewSetter = fieldInfo.CreateSetter();
                    ViewGetter = fieldInfo.CreateGetter();
                    break;
                case MemberTypes.Property:
                    var propertyInfo = memberInfo as PropertyInfo;
//                    ViewSetter = value => propertyInfo?.SetValue(component, value);
//                    ViewGetter = () => propertyInfo?.GetValue(component);

                    ViewSetter = propertyInfo.CreateSetter();
                    ViewGetter = propertyInfo.CreateGetter();
                    break;
                default:
                    throw new Exception(
                        $"MemberType: {memberInfo.MemberType} is not supported in one way property binding.");
            }

            #endregion

            #region View Model

            memberInfo = ParseViewModelEntry(ViewModel, ViewModelEntry);
            ViewModelInstance = ViewModel;
            ViewModelMemberName = memberInfo.Name;

            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    var fieldInfo = memberInfo as FieldInfo;
//                    ViewModelGetter = () => fieldInfo?.GetValue(ViewModel);
//                    ViewModelSetter = value => fieldInfo?.SetValue(ViewModel, value);
                    ViewModelGetter = fieldInfo.CreateGetter();
                    ViewModelSetter = fieldInfo.CreateSetter();
                    break;
                case MemberTypes.Property:
                    var propertyInfo = memberInfo as PropertyInfo;
//                    ViewModelGetter = () => propertyInfo?.GetValue(ViewModel);
//                    ViewModelSetter = value => propertyInfo?.SetValue(ViewModel, value);
                    ViewModelGetter = propertyInfo.CreateGetter();
                    ViewModelSetter = propertyInfo.CreateSetter();
                    
                    break;
                default:
                    throw new Exception(
                        $"MemberType: {memberInfo.MemberType} is not supported in one way property binding.");
            }

            #endregion
        }
    }
}