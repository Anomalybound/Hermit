using System;
using System.Reflection;
using Hermit.DataBinding;
using UnityEngine;
using UnityEngine.Events;

namespace Hermit
{
    public class TwoWayPropertyBinding : OneWayPropertyBinding
    {
        [SerializeField]
        private string _viewEventEntry;

        [SerializeField]
        private string viewModelAdapterType;

        [SerializeField]
        private AdapterOptions _viewModelAdapterOptions;

        #region Properties

        public string ViewEventEntry
        {
            get => _viewEventEntry;
            set
            {
#if UNITY_EDITOR
                if (_viewEventEntry != value) { UnityEditor.EditorUtility.SetDirty(this); }
#endif
                _viewEventEntry = value;
            }
        }

        public string ViewModelAdapterType
        {
            get => viewModelAdapterType;
            set
            {
#if UNITY_EDITOR
                if (viewModelAdapterType != value) { UnityEditor.EditorUtility.SetDirty(this); }
#endif
                viewModelAdapterType = value;
            }
        }

        public AdapterOptions ViewModelAdapterOptions
        {
            get => _viewModelAdapterOptions;
            set
            {
#if UNITY_EDITOR
                if (_viewModelAdapterOptions != value) { UnityEditor.EditorUtility.SetDirty(this); }
#endif
                _viewModelAdapterOptions = value;
            }
        }

        #endregion

        #region Runtime Variables

        protected Action ViewChangedAction;

        protected IEventBinder ViewEventBinder;

        protected IAdapter ViewModelAdapter;

        #endregion

        public override void SetupBinding()
        {
            base.SetupBinding();

            BindViewModel2ViewEvent();

            GetViewModelAdapterInstance();
        }

        public override void UpdateBinding()
        {
            base.UpdateBinding();

            UpdateViewProperty();
        }

        public override void Connect()
        {
            base.Connect();

            ViewEventBinder?.Connect();

            UpdateViewProperty();
        }

        public override void Disconnect()
        {
            base.Disconnect();

            ViewEventBinder?.Disconnect();
        }

        protected void GetViewModelAdapterInstance()
        {
            if (string.IsNullOrEmpty(viewModelAdapterType)) { return; }

            ViewModelAdapter = Her.Resolve<IAdapter>(viewModelAdapterType);
        }

        protected void BindViewModel2ViewEvent()
        {
            ViewChangedAction = UpdateViewProperty;

            #region View Events

            var (component, memberInfo) = ParseViewEntry(this, ViewEventEntry);

            switch (memberInfo.MemberType)
            {
                case MemberTypes.Event:
                    var eventInfo = memberInfo as EventInfo;
                    ViewEventBinder = EventBinderBase.CreateEventBinder(component, eventInfo, ViewChangedAction);
                    break;
                case MemberTypes.Field:
                    var fieldInfo = memberInfo as FieldInfo;
                    var fieldEventInstance = fieldInfo?.GetValue(component);
                    if (fieldEventInstance is UnityEventBase)
                    {
                        ViewEventBinder = EventBinderBase.CreateUnityEventBinder(fieldEventInstance,
                            fieldInfo.FieldType, ViewChangedAction);
                    }

                    break;
                case MemberTypes.Property:
                    var propertyInfo = memberInfo as PropertyInfo;
                    var propertyEventInstance = propertyInfo?.GetValue(component);
                    if (propertyEventInstance is UnityEventBase)
                    {
                        ViewEventBinder = EventBinderBase.CreateUnityEventBinder(propertyEventInstance,
                            propertyInfo.PropertyType, ViewChangedAction);
                    }

                    break;
                default:
                    throw new Exception(
                        $"MemberType: {memberInfo.MemberType} is not supported in two way property binding.");
            }

            #endregion
        }

        protected virtual void UpdateViewProperty()
        {
            PropertyChanging = true;

            var rawValue = ViewGetter.Invoke();
            var convertedValue = ViewModelAdapterOptions != null
                ? ViewAdapterInstance?.Convert(rawValue, ViewModelAdapterOptions)
                : ViewAdapterInstance?.Convert(rawValue);
            
            ViewModelSetter.Invoke(ViewModelAdapter != null ? convertedValue : rawValue);

            PropertyChanging = false;
        }
    }
}