using System;
using System.Collections.Specialized;
using System.Reflection;
using Hermit.Common.DataBinding.Core;
using UnityEngine;

namespace Hermit.Common.DataBinding
{
    public class CollectionBinding : DataBindingBase
    {
        [SerializeField] private string viewModelCollectionEntry;

        [SerializeField] private string collectionHandlerTypeName;

        [SerializeField] private GameObject viewTemplate;

        [SerializeField] private Transform viewContainer;

        #region Propertyies

        public string ViewModelCollectionEntry
        {
            get => viewModelCollectionEntry;
            set
            {
#if UNITY_EDITOR
                if (viewModelCollectionEntry != value) { UnityEditor.EditorUtility.SetDirty(this); }
#endif
                viewModelCollectionEntry = value;
            }
        }

        public string CollectionHandlerTypeName
        {
            get => collectionHandlerTypeName;
            set
            {
#if UNITY_EDITOR
                if (collectionHandlerTypeName != value) { UnityEditor.EditorUtility.SetDirty(this); }
#endif
                collectionHandlerTypeName = value;
            }
        }

        public GameObject ViewTemplate
        {
            get => viewTemplate;
            set
            {
#if UNITY_EDITOR
                if (viewTemplate != value) { UnityEditor.EditorUtility.SetDirty(this); }
#endif
                viewTemplate = value;
            }
        }


        public Transform ViewContainer
        {
            get
            {
                if (viewContainer == null) { return viewTemplate != null ? viewTemplate.transform.parent : null; }

                return viewContainer;
            }
            set
            {
#if UNITY_EDITOR
                if (viewContainer != value) { UnityEditor.EditorUtility.SetDirty(this); }
#endif
                viewContainer = value;
            }
        }

        #endregion

        #region Runtime Variables

        protected INotifyCollectionChanged CollectionValue;

        protected IViewCollectionChangedHandler ChangedHandler;

        #endregion

        public override void SetupBinding()
        {
            base.SetupBinding();

            GetCollectionChangedHandlerInstance();

            BindViewModelCollections();
        }

        private void GetCollectionChangedHandlerInstance()
        {
            if (string.IsNullOrEmpty(CollectionHandlerTypeName)) { return; }

            ChangedHandler = Her.Resolve<IViewCollectionChangedHandler>(CollectionHandlerTypeName);
            ChangedHandler.SetUp(ViewTemplate, viewContainer);
        }

        public override void Connect()
        {
            base.Connect();

            if (CollectionValue == null) { return; }

            CollectionValue.CollectionChanged += ChangedHandler.OnCollectionChanged;
        }

        public override void Disconnect()
        {
            base.Disconnect();

            if (CollectionValue == null) { return; }

            CollectionValue.CollectionChanged -= ChangedHandler.OnCollectionChanged;
        }

        public override void UpdateBinding() { }

        private void BindViewModelCollections()
        {
            #region View Model

            var memberInfo = ParseViewModelEntry(ViewModel, ViewModelCollectionEntry);

            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    var fieldInfo = (FieldInfo) memberInfo;
                    CollectionValue = fieldInfo.GetValue(ViewModel) as INotifyCollectionChanged;
                    break;
                case MemberTypes.Property:
                    var propertyInfo = (PropertyInfo) memberInfo;
                    CollectionValue = propertyInfo.GetValue(ViewModel) as INotifyCollectionChanged;
                    break;
                default:
                    throw new Exception(
                        $"MemberType: {memberInfo.MemberType} is not supported in one way property binding.");
            }

            #endregion
        }
    }
}