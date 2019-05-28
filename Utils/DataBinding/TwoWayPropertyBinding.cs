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
                UnityEditor.EditorUtility.SetDirty(this);
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
                UnityEditor.EditorUtility.SetDirty(this);
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
                UnityEditor.EditorUtility.SetDirty(this);
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

        protected override void Awake()
        {
            base.Awake();

            BindViewModel2ViewEvent();

            SetupViewModelAdapter();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            ViewEventBinder?.Connect();

            UpdateViewProperty();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            ViewEventBinder?.Disconnect();
        }

        protected void SetupViewModelAdapter()
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
                    CreateEventBinder(component, eventInfo);
                    break;
                case MemberTypes.Field:
                    var fieldInfo = memberInfo as FieldInfo;
                    var fieldEventInstance = fieldInfo?.GetValue(component);
                    if (fieldEventInstance is UnityEventBase)
                    {
                        CreateUnityEventBinder(fieldEventInstance, fieldInfo.FieldType);
                    }

                    break;
                case MemberTypes.Property:
                    var propertyInfo = memberInfo as PropertyInfo;
                    var propertyEventInstance = propertyInfo?.GetValue(component);
                    if (propertyEventInstance is UnityEventBase)
                    {
                        CreateUnityEventBinder(propertyEventInstance, propertyInfo.PropertyType);
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
            var convertedValue = ViewModelAdapter?.Covert(rawValue, ViewModelAdapterOptions);
            ViewModelSetter.Invoke(ViewModelAdapter != null ? convertedValue : rawValue);

            PropertyChanging = false;
        }

        #region Helpers

        public void CreateEventBinder(Component target, EventInfo eventInfo)
        {
            var handlerType = eventInfo.EventHandlerType;
            var valueArguments = handlerType.GetGenericArguments();

            var eventBinderType = valueArguments.Length <= 0
                ? typeof(EventBinder)
                : typeof(EventBinder<>).MakeGenericType(valueArguments);

            ViewEventBinder = Activator.CreateInstance(eventBinderType, eventInfo, target,
                ViewChangedAction) as IEventBinder;
        }

        public void CreateUnityEventBinder(object eventInstance, Type eventType)
        {
            // UnityEvent<T0> event
            var unityEventType = GetUnityEventType(eventType);
            if (unityEventType == null) { throw new Exception($"Event {eventInstance} is not an UnityEvent."); }

            var valueArguments = unityEventType.GetGenericArguments();
            var unityEventBinderType = valueArguments.Length <= 0
                ? typeof(UnityEventBinder)
                : typeof(UnityEventBinder<>).MakeGenericType(valueArguments);

            ViewEventBinder = Activator.CreateInstance(unityEventBinderType, eventInstance,
                ViewChangedAction) as IEventBinder;
        }

        private static Type GetUnityEventType(Type type)
        {
            var ret = type;
            while (ret.BaseType != null)
            {
                if (ret.BaseType == typeof(UnityEventBase)) { return ret; }

                ret = ret.BaseType;
            }

            return null;
        }

        #endregion
    }
}