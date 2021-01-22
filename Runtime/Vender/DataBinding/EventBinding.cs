using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace Hermit.DataBinding
{
    [AddComponentMenu("Hermit/Data Binding/Event Binding")]
    public class EventBinding : DataBindingBase
    {
        [SerializeField] private string viewEventEntry;

        [SerializeField] private string viewModelActionEntry;

        #region Properties

        public string ViewEventEntry
        {
            get => viewEventEntry;
            set
            {
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
                viewEventEntry = value;
            }
        }

        public string ViewModelActionEntry
        {
            get => viewModelActionEntry;
            set
            {
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
                viewModelActionEntry = value;
            }
        }

        #endregion

        #region Runtime Variables

        protected IEventBinder ViewEventBinder;

        #endregion

        protected virtual void BindView2ViewModel()
        {
            #region ViewModel Actions

            var actionMethod = ParseViewModelEntry(ViewModel, ViewModelActionEntry);

            var viewModelAction = DelegateHelper.CreateInstance(ViewModel, actionMethod as MethodInfo);

            #endregion

            #region View Events

            var (component, memberInfo) = ParseViewEntry(this, ViewEventEntry);

            switch (memberInfo.MemberType)
            {
                case MemberTypes.Event:
                    var eventInfo = memberInfo as EventInfo;
                    ViewEventBinder = EventBinderBase.CreateEventBinder(component, eventInfo, viewModelAction);
                    break;
                case MemberTypes.Field:
                    var fieldInfo = memberInfo as FieldInfo;
                    var fieldEventInstance = fieldInfo?.GetValue(component);
                    if (fieldEventInstance is UnityEventBase)
                    {
                        ViewEventBinder = EventBinderBase.CreateUnityEventBinder(fieldEventInstance,
                            fieldInfo.FieldType, viewModelAction);
                    }

                    break;
                case MemberTypes.Property:
                    var propertyInfo = memberInfo as PropertyInfo;
                    var propertyEventInstance = propertyInfo?.GetValue(component);
                    if (propertyEventInstance is UnityEventBase)
                    {
                        ViewEventBinder = EventBinderBase.CreateUnityEventBinder(propertyEventInstance,
                            propertyInfo.PropertyType, viewModelAction);
                    }

                    break;
                default:
                    throw new Exception(
                        $"MemberType: {memberInfo.MemberType} is not supported in event binding.");
            }

            #endregion
        }

        public override void SetupBinding()
        {
            base.SetupBinding();

            BindView2ViewModel();
        }

        public override void Connect()
        {
            base.Connect();

            ViewEventBinder?.Connect();
        }

        public override void Disconnect()
        {
            base.Disconnect();

            ViewEventBinder?.Disconnect();
        }

        public override void UpdateBinding() { }
    }
}