using System;
using System.Reflection;
using Hermit.DataBinding;
using UnityEngine;
using UnityEngine.Events;

namespace Hermit
{
    public class EventBinding : DataBindingBase
    {
        [SerializeField]
        private string _viewEventEntry;

        [SerializeField]
        private string _viewModelActionEntry;

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

        public string ViewModelActionEntry
        {
            get => _viewModelActionEntry;
            set
            {
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
                _viewModelActionEntry = value;
            }
        }

        #endregion

        #region Runtime Variables

        protected IEventBinder ViewEventBinder;

        #endregion

        protected virtual void BindViewAction2ViewModelFunction()
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
                        $"MemberType: {memberInfo.MemberType} is not supported in two way property binding.");
            }

            #endregion
        }

        public override void SetupBinding()
        {
            base.SetupBinding();
            
            BindViewAction2ViewModelFunction();
        }

        public override void Connect()
        {
            ViewEventBinder?.Connect();
        }

        public override void Disconnect()
        {
            ViewEventBinder?.Disconnect();
        }

        public override void UpdateBinding() { }
    }
}