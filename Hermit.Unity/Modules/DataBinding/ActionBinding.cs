using System;
using System.Reflection;
using Hermit.DataBinding;
using UnityEngine;

namespace Hermit
{
    [AddComponentMenu("Hermit/Data Binding/Action Binding")]
    public class ActionBinding : DataBindingBase
    {
        public bool showDeclaredMethodsOnly = true;

        [SerializeField] private string viewActionEntry;

        [SerializeField] private string viewModelEventEntry;

        #region Properties

        public string ViewActionEntry
        {
            get => viewActionEntry;
            set
            {
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
                viewActionEntry = value;
            }
        }

        public string ViewModelEventEntry
        {
            get => viewModelEventEntry;
            set
            {
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
                viewModelEventEntry = value;
            }
        }

        #endregion

        #region Runtime Variables

        protected IEventBinder ViewEventBinder;
        protected Delegate MethodDelegate;

        #endregion

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

        protected void BindView2ViewModel()
        {
            #region View 

            var (component, memberInfo) = ParseViewEntry(this, ViewActionEntry);

            switch (memberInfo.MemberType)
            {
                case MemberTypes.Method:
                    var methodInfo = memberInfo as MethodInfo;
                    MethodDelegate = DelegateHelper.CreateInstance(component, methodInfo);
                    break;

                default:
                    throw new Exception($"MemberType: {memberInfo.MemberType} is not supported in action binding.");
            }

            #endregion

            #region View Model

            memberInfo = ParseViewModelEntry(ViewModel, ViewModelEventEntry);

            switch (memberInfo.MemberType)
            {
                case MemberTypes.Event:
                    var eventInfo = memberInfo as EventInfo;
                    ViewEventBinder = EventBinderBase.CreateEventBinder(ViewModel, eventInfo, MethodDelegate);
                    break;
                default:
                    throw new Exception($"MemberType: {memberInfo.MemberType} is not supported in action binding.");
            }

            #endregion
        }
    }
}