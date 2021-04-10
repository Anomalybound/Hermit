using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace Hermit.DataBinding
{
    [AddComponentMenu("Hermit/Data Binding/Two-way Binding")]
    public class TwoWayPropertyBinding : OneWayPropertyBinding
    {
        [SerializeField] private string viewEventEntry;

        [SerializeField] private string viewModelAdapterTypeString;

        [SerializeField] private AdapterOptions viewModelAdapterOptions;

        #region Properties

        public string ViewEventEntry
        {
            get => viewEventEntry;
            set
            {
#if UNITY_EDITOR
                if (viewEventEntry != value) { UnityEditor.EditorUtility.SetDirty(this); }
#endif
                viewEventEntry = value;
            }
        }

        public string ViewModelAdapterTypeString
        {
            get => viewModelAdapterTypeString;
            set
            {
#if UNITY_EDITOR
                if (viewModelAdapterTypeString != value) { UnityEditor.EditorUtility.SetDirty(this); }
#endif
                viewModelAdapterTypeString = value;
            }
        }

        public AdapterOptions ViewModelAdapterOptions
        {
            get => viewModelAdapterOptions;
            set
            {
#if UNITY_EDITOR
                if (viewModelAdapterOptions != value) { UnityEditor.EditorUtility.SetDirty(this); }
#endif
                viewModelAdapterOptions = value;
            }
        }

        #endregion

        #region Runtime Variables

        protected Action ViewChangedAction;

        protected IEventBinder ViewEventBinder;

        protected IAdapter ViewModelAdapterInstance;

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

            UpdateViewModelProperty();
        }

        public override void Connect()
        {
            base.Connect();

            ViewEventBinder?.Connect();

            UpdateViewModelProperty();
        }

        public override void Disconnect()
        {
            base.Disconnect();

            ViewEventBinder?.Disconnect();
        }

        protected void GetViewModelAdapterInstance()
        {
            if (string.IsNullOrEmpty(viewModelAdapterTypeString)) { return; }

            ViewModelAdapterInstance = App.Resolve<IAdapter>(viewModelAdapterTypeString);
        }

        protected void BindViewModel2ViewEvent()
        {
            ViewChangedAction = UpdateViewModelProperty;

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

        protected virtual void UpdateViewModelProperty()
        {
            PropertyChanging = true;

            var rawValue = ViewGetter.Invoke(ComponentInstance);
            var convertedValue = ViewModelAdapterOptions != null
                ? ViewModelAdapterInstance?.Convert(rawValue, ViewModelAdapterOptions)
                : ViewModelAdapterInstance?.Convert(rawValue);

            ViewModelSetter.Invoke(ViewModelInstance, ViewModelAdapterInstance != null ? convertedValue : rawValue);
            PropertyChanging = false;
        }
    }
}