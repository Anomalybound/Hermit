using System;
using System.ComponentModel;
using System.Reflection;
using Hermit.DataBinding;
using UnityEngine;
using Component = UnityEngine.Component;

namespace Hermit
{
    public class OneWayPropertyBinding : DataBindingBase
    {
        public Component DataProvider;

        [SerializeField]
        private string _viewModelEntry;

        [SerializeField]
        private string _viewEntry;

        [SerializeField]
        private string _adapterType;

        [SerializeField]
        private AdapterOptions _adapterOptions;

        #region Properties

        public string ViewModelEntry
        {
            get => _viewModelEntry;
            set
            {
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
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
                UnityEditor.EditorUtility.SetDirty(this);
#endif
                _viewEntry = value;
            }
        }

        public string AdapterType
        {
            get => _adapterType;
            set
            {
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
                _adapterType = value;
            }
        }

        public AdapterOptions AdapterOptions
        {
            get => _adapterOptions;
            set => _adapterOptions = value;
        }

        #endregion

        #region Runtime Variables

        private ViewModel _viewModel;

        private string _viewModelMemberName;

        private IAdapter _adapterInstance;

        private Action<object> _viewSetter;

        private Func<object> _viewModelGetter;

        #endregion

        protected void Awake()
        {
            if (DataProvider is IViewModelProvider provider) { _viewModel = provider.GetViewModel(); }

            if (string.IsNullOrEmpty(_adapterType)) { return; }

            _adapterInstance = Her.Resolve<IAdapter>(_adapterType);
        }

        private void OnEnable()
        {
            if (_viewModel != null) { _viewModel.PropertyChanged += OnPropertyChanged; }

            Bind(ViewEntry, ViewModelEntry);
        }

        private void OnDisable()
        {
            if (_viewModel != null) { _viewModel.PropertyChanged -= OnPropertyChanged; }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_viewModelMemberName != e.PropertyName) { return; }

            var rawValue = _viewModelGetter.Invoke();
            var convertedValue = _adapterInstance?.Covert(rawValue, _adapterOptions);
            _viewSetter.Invoke(_adapterInstance != null ? convertedValue : rawValue);
        }

        private void Bind(string viewPropertyName, string viewModelPropertyName)
        {
            #region View 

            var (typeName, memberName) = ParseEndPointReference(viewPropertyName);

            var component = GetComponent(typeName);
            if (component == null) { throw new Exception($"Can't find component of type: {typeName}."); }

            var viewMemberInfos = component.GetType().GetMember(memberName);
            if (viewMemberInfos.Length <= 0)
            {
                throw new Exception($"Can't find member of name: {memberName} on {component}.");
            }

            var memberInfo = viewMemberInfos[0];

            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    var fieldInfo = memberInfo as FieldInfo;
                    _viewSetter = value => fieldInfo?.SetValue(component, value);
                    break;
                case MemberTypes.Property:
                    var propertyInfo = memberInfo as PropertyInfo;
                    _viewSetter = value => propertyInfo?.SetValue(component, value);
                    break;
                default:
                    throw new Exception($"MemberType: {memberName} is not supported in one way property binding.");
            }

            #endregion

            #region View Model

            (_, _viewModelMemberName) = ParseEndPointReference(viewModelPropertyName);

            var viewModelMemberInfos = _viewModel.GetType().GetMember(_viewModelMemberName);
            if (viewModelMemberInfos.Length <= 0)
            {
                throw new Exception($"Can't find member of name: {_viewModelMemberName} on {_viewModel}.");
            }

            memberInfo = viewModelMemberInfos[0];

            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    var fieldInfo = memberInfo as FieldInfo;
                    _viewModelGetter = () => fieldInfo?.GetValue(_viewModel);
                    break;
                case MemberTypes.Property:
                    var propertyInfo = memberInfo as PropertyInfo;
                    _viewModelGetter = () => propertyInfo?.GetValue(_viewModel);
                    break;
                default:
                    throw new Exception($"MemberType: {memberName} is not supported in one way property binding.");
            }

            #endregion
        }
    }
}