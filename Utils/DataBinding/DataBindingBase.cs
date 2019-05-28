using System;
using System.ComponentModel;
using System.Reflection;
using Hermit.DataBinding;
using UnityEngine;
using Component = UnityEngine.Component;

namespace Hermit
{
    public abstract class DataBindingBase : MonoBehaviour
    {
        public Component DataProvider;

        #region Runtime Variables

        protected ViewModel ViewModel;

        #endregion

        #region Helpers

        protected virtual void Awake()
        {
            if (DataProvider is IViewModelProvider provider) { ViewModel = provider.GetViewModel(); }
        }

        protected static (string typeName, string memberName) ParseEntryTypeMember(string entry)
        {
            var lastPeriodIndex = entry.LastIndexOf('.');
            if (lastPeriodIndex == -1) { throw new Exception($"No period was found: {entry}"); }

            var typeName = entry.Substring(0, lastPeriodIndex);
            var memberName = entry.Substring(lastPeriodIndex + 1);

            //Due to (undocumented) unity behaviour, some of their components do not work with the namespace when using GetComponent(""), and all of them work without the namespace
            //So to be safe, we remove all namespaces from any component that starts with UnityEngine
            if (typeName.StartsWith("UnityEngine.")) { typeName = typeName.Substring(typeName.LastIndexOf('.') + 1); }

            if (typeName.Length == 0 || memberName.Length == 0)
            {
                throw new Exception($"Bad formatting! Expected [<type-name>.<member-name>]: {entry} ");
            }

            return (typeName, memberName);
        }

        protected static MemberInfo ParseViewModelEntry(ViewModel viewModel, string entry)
        {
            var (_, memberName) = ParseEntryTypeMember(entry);

            var viewMemberInfos = viewModel.GetType().GetMember(memberName);
            if (viewMemberInfos.Length <= 0)
            {
                throw new Exception($"Can't find member of name: {memberName} on {viewModel}.");
            }

            var memberInfo = viewMemberInfos[0];
            return memberInfo;
        }

        protected static (Component component, MemberInfo memberInfo) ParseViewEntry(Component viewProvider,
            string entry)
        {
            var (typeName, memberName) = ParseEntryTypeMember(entry);

            var component = viewProvider.GetComponent(typeName);
            if (component == null)
            {
                throw new Exception($"Can't find component of type: {typeName} on {viewProvider}.");
            }

            var viewMemberInfos = component.GetType().GetMember(memberName);
            if (viewMemberInfos.Length <= 0)
            {
                throw new Exception($"Can't find member of name: {memberName} on {component}.");
            }

            var memberInfo = viewMemberInfos[0];

            return (component, memberInfo);
        }

        #endregion
    }
}