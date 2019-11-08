using System;
using System.Reflection;
using UnityEngine;
using Component = UnityEngine.Component;

namespace Hermit
{
    [ScriptOrder(-2000)]
    public abstract class DataBindingBase : MonoBehaviour
    {
        public Component dataProviderComponent;

        #region Runtime Variables

        protected ViewModel ViewModel;

        protected bool IsDataReady;

        protected bool IsBindingConnected;

        protected IViewModelProvider DataProvider;

        #endregion

        #region Helpers

        protected virtual void Awake()
        {
            if (dataProviderComponent != null && dataProviderComponent is IViewModelProvider provider)
            {
                DataProvider = provider;
                if (DataProvider.GetViewModel() != null)
                {
                    IsDataReady = true;
                    SetupBinding();
                }
                else { DataProvider.DataReadyEvent += OnDataReady; }
            }
            else
            {
                DataProvider = GetComponentInParent<IViewModelProvider>();

                if (DataProvider.GetViewModel() != null)
                {
                    IsDataReady = true;
                    SetupBinding();
                }
                else { DataProvider.DataReadyEvent += OnDataReady; }
            }
        }

        protected virtual void OnDestroy()
        {
            if (DataProvider != null) { DataProvider.DataReadyEvent -= OnDataReady; }
        }

        protected virtual void OnEnable()
        {
            if (!IsDataReady) { return; }

            if (IsBindingConnected) { return; }

            Connect();
        }

        protected virtual void OnDisable()
        {
            if (!IsDataReady) { return; }

            if (!IsBindingConnected) { return; }

            Disconnect();
        }

        private void OnDataReady()
        {
            if (!IsDataReady)
            {
                IsDataReady = true;
                SetupBinding();
            }

            if (!enabled || IsBindingConnected) { return; }

            Connect();
        }

        public virtual void SetupBinding()
        {
            ViewModel = DataProvider.GetViewModel();
        }

        public virtual void Connect()
        {
            IsBindingConnected = true;
        }

        public virtual void Disconnect()
        {
            IsBindingConnected = false;
        }

        public abstract void UpdateBinding();

        protected (string typeName, string memberName) ParseEntry2TypeMember(string entry)
        {
            var lastPeriodIndex = entry.LastIndexOf('.');
            if (lastPeriodIndex == -1) { throw new Exception($"No period was found[{entry}] on {name}"); }

            var typeName = entry.Substring(0, lastPeriodIndex);
            var memberName = entry.Substring(lastPeriodIndex + 1);

            //Due to (undocumented) unity behaviour, some of their components do not work with the namespace when using GetComponent(""), and all of them work without the namespace
            //So to be safe, we remove all namespaces from any component that starts with UnityEngine
            if (typeName.StartsWith("UnityEngine.")) { typeName = typeName.Substring(typeName.LastIndexOf('.') + 1); }

            if (typeName.Length == 0 || memberName.Length == 0)
            {
                Her.Error($"Bad formatting! Expected [<type-name>.<member-name>]: {entry} ");
                return (null, null);
            }

            return (typeName, memberName);
        }

        protected MemberInfo ParseViewModelEntry(ViewModel viewModel, string entry)
        {
            var (_, memberName) = ParseEntry2TypeMember(entry);

            var viewMemberInfos = viewModel.GetType().GetMember(memberName);
            if (viewMemberInfos.Length <= 0)
            {
                Her.Error($"Can't find member of name: {memberName} on {viewModel}.");
                return null;
            }

            var memberInfo = viewMemberInfos[0];
            return memberInfo;
        }

        protected (Component component, MemberInfo memberInfo) ParseViewEntry(Component viewProvider,
            string entry)
        {
            var (typeName, memberName) = ParseEntry2TypeMember(entry);

            var component = viewProvider.GetComponent(typeName);
            if (component == null)
            {
                Her.Error($"Can't find component of type: {typeName} on {viewProvider}.");
                return (null, null);
            }

            var viewMemberInfos = component.GetType().GetMember(memberName);
            if (viewMemberInfos.Length <= 0)
            {
                Her.Error($"Can't find member of name: {memberName} on {component}.");
                return (null, null);
            }

            var memberInfo = viewMemberInfos[0];

            return (component, memberInfo);
        }

        #endregion
    }
}